using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vm_cs_fresh
{
    public partial class Form1 : Form
    {
        const int FRAME_SIZE = 256; //Frame is 256 bytes
        const int PAGE_SIZE = 256; //Page is 256 bytes
        const int RAM_SIZE = 128; //RAM holds 128 frames

        const int TLB_SIZE = 16; //TLB has space for 16 frames
        const int PAGE_TBL_SIZE = 128; //Holds 256 pages.

        string addressFileLoaded; //path to input addresses file loaded
        string binFileLoaded; //path to input address for binary file

        string[] addressFileContents = new string[1000]; //contents of input "addresses" file
        double[] backingStoreContents = new double[65536]; //Contents of backing store BIN file.

        //Contains physical addresses corresponding to input file before program is run
        //and stuff is in tables, etc.
        string[] oldPhysicalAddresses = new string[1000];

        //Contains physical addresses corresponding to input file after program is run
        //and stuff is in tables, etc.
        string[] newPhysicalAddresses = new string[1000];

        //Contains values that addresses point to, pretending there is only a backing store
        //(before program is run).
        string[] noTableValues = new string[1000];

        //Contains values that addresses obtain, pretending there are tables and a backing store.
        //(once program is run with tables).  Hopefully, this will match 'noTableValues'
        string[] withTableValues = new string[1000];

        //Translation lookaside buffer (TLB) and page table, and RAM
        double[,] TLB = new double[16, 2]; //[page #, frame #]
        double[,] pageTable = new double[128, 2]; //[page #, frame #]
        double[,] RAM = new double[128, 256]; //Each frame's data is store in that frame's row of RAM.

        //Next pages table and TLB indexes to be replaced.
        double tlbNextToGo = 0;
        double pageTblNextToGo = 0;

        //# of hits and faults on page table and TLB to report statistics later.
        double numPageTblFaults = 0;
        double numPageTblHits = 0;
        double numTlbFaults = 0;
        double numTlbHits = 0;

        //FUNCTIONS FOR REPEATED OPERATIONS AND GROUPING:

        //Converts a decimal number to binary: Little-Endian
        double[] ToBinary(double decNum)
        {
            double decNumLeft = decNum; //Decimal value left to be filled with binary places.

            //If decNum was in binary, highest place value.
            double highestBinIndex = Math.Floor(Math.Log(decNum, 2));

            //Accumulates binary digits to return, in Little Endian.
            double[] binaryHolder = new double[16];
            try
            {
                binaryHolder = new double[Convert.ToInt32(highestBinIndex) + 1];
            }
            catch (System.OverflowException)
            {
                //MessageBox.Show(decNum.ToString());
            }


            //Goes through all binary places 'decNum' could have once in binary, starting from the
            //highest place possible
            for (int onBinIndex = Convert.ToInt32(highestBinIndex); onBinIndex >= 0; onBinIndex--)
            {
                //Does decNum have a 1 or 0 in 'onBinIndex' binary place.
                double oneOrZero = Math.Floor(decNumLeft / Math.Pow(2, onBinIndex));

                //Puts one or zero in current position in holder, 'onBinIndex'
                binaryHolder[onBinIndex] = Convert.ToInt32(oneOrZero);

                //If 'onBinIndex' is 1, some of the decimal input has been
                //taken care of.
                if (oneOrZero == 1)
                {
                    decNumLeft -= Math.Pow(2, onBinIndex);
                }
            }

            //Returns final binary value.
            return binaryHolder;
        }

        double ToDecimal(double[] binNum)
        {
            //Goes through binary digits and adds their values
            //to a decimal number being built.
            double decInProgress = 0;

            //Loop that goes through binary digits
            for (int onBinDigit = 0; onBinDigit < binNum.Length; onBinDigit++)
            {
                //Adds digit's value if it is 1, not 0.
                decInProgress += binNum[onBinDigit] * Math.Pow(2, onBinDigit);
            }

            return decInProgress;
        }

        ///Replaces something in the TLB if just found in page table: new "pageNum", new "frameNum", and "index"
        ///to put it in.
        void TLBreplace(double pageNum, double frameNum)
        {
            //What index in TLB to replace according to FIFO tracking.
            double tlbIndexUsed = tlbNextToGo;

            //Adds page to TLB.
            TLB[Convert.ToInt32(tlbIndexUsed), 0] = pageNum;
            TLB[Convert.ToInt32(tlbIndexUsed), 1] = frameNum;

            //FIFO tracking of TLB management.  Increments "next victim index"
            //through 0 - 15.
            if(tlbNextToGo == 15)
            {
                tlbNextToGo = 0;
            }
            else
            {
                tlbNextToGo++;
            } 
        }

        ///Replaces something in the page table if it is not there.
        ///Returns victim frame.
        double PageTblReplace(double pageNum)
        {
            //With FIFO algorithm, what's next page index to be replaced?
            double pageIndexUsed = pageTblNextToGo;

            //Adds page to page table. Say what page number and frame number is.
            pageTable[Convert.ToInt32(pageIndexUsed), 0] = pageNum;
            pageTable[Convert.ToInt32(pageIndexUsed), 1] = pageIndexUsed;

            //Loading page in to RAM.
            ramReplace(pageTable[Convert.ToInt32(pageIndexUsed), 1], pageNum);

            //Increments to next FIFO index that will get replaced.
            //If 127 (greatest page table index), "next index to go"
            //resets
            if (pageTblNextToGo == 127)
            {
                pageTblNextToGo = 0;
            }
            else
            {
                pageTblNextToGo++;
            }

            return pageIndexUsed; //Returns victim frame
        }

        ///Replaces a frame in RAM:
        void ramReplace(double frameNum, double pageNum)
        {
            for (int onByte = 0; onByte < FRAME_SIZE; onByte++)
            {
                RAM[Convert.ToInt32(frameNum), onByte] = backingStoreContents[Convert.ToInt32(pageNum) * FRAME_SIZE + onByte];
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Opens list of logical addresses initially to deal with
            addressFileLoaded = "C:\\Quick\\addresses.txt";
            addressFileContents = System.IO.File.ReadAllLines(addressFileLoaded);

            //Load backing store contents
            //Location of backing store binary file.
            binFileLoaded = "C:\\Quick\\BACKING_STORE.BIN";

            //Loads in all bytes of binary file
            byte[] binFile = System.IO.File.ReadAllBytes(binFileLoaded);

            //Converts all bytes to strings, storing them in 'backingStoreContents'
            for (int onLine = 0; onLine < 65536; onLine++)
            {
                backingStoreContents[onLine] = double.Parse(binFile[onLine].ToString());
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            //For processing progress tracking.
            operationProgress.Maximum = 1000;

            //Going through all of addresses.txt (it has 1000 lines)
            for (int onLine = 0; onLine < 1000; onLine++)
            {
                //Current line to decimal then to binary
                double currentLine = double.Parse(addressFileContents[onLine]);
                double[] currentLineBin = ToBinary(currentLine);

                //Gets number of places in current line in binary.
                int addressNumPlaces = currentLineBin.Length;

                double[] pageNum = new double[8];
                double[] offset = new double[8];

                //Goes through all digits of address on line.
                for (int onDigit = 0; onDigit < addressNumPlaces; onDigit++)
                {
                    //Process each binary digit in line
                    if (onDigit < 8) //Digits 0-7 are the offset.
                    {
                        offset[onDigit] = currentLineBin[onDigit];
                    }
                    else if (onDigit > 7) //Digits 8-15 are the page number.
                    {
                        pageNum[onDigit - 8] = currentLineBin[onDigit];
                    }
                }

                //Physical address when only backing store's being considered.
                double noTblPhysicalAddress = onLine * PAGE_SIZE + ToDecimal(offset);

                //Adds physical address to list of non-tabled ones.
                oldPhysicalAddresses[onLine] = noTblPhysicalAddress.ToString();

                //Value in backing store (no tables are ever being used yet)
                double value = backingStoreContents[Convert.ToInt32(ToDecimal(pageNum))
                    * PAGE_SIZE + Convert.ToInt32(ToDecimal(offset))];

                //Puts value in list of non-tabled values.
                noTableValues[onLine] = value.ToString();

                //SECTION FOR CHECKING TABLES:

                //Value which will be obtained from table or backing store.
                double valueObtained = 0;

                //If obtained from table, what is the frame number?
                double frameObtained = 1000;

                //Checking TLB:
                bool tlbHit = false; //False until proven true

                //Will store TLB index where page is found if it is found
                int tlbFoundWhere = 1000;

                //Going through TLB
                for (int onTlbIndex = 0; onTlbIndex < TLB_SIZE; onTlbIndex++)
                {
                    //Is there a match in the page # column?
                    if (TLB[onTlbIndex, 0] == ToDecimal(pageNum))
                    {
                        tlbHit = true;
                        tlbFoundWhere = onTlbIndex;
                    }
                }

                //MessageBox.Show("Line: " + onLine.ToString() + ", This page: " + ToDecimal(pageNum).ToString());

                //Is there a TLB hit?
                if (tlbHit)
                {
                    //Tracking TLB hits.
                    numTlbHits++;

                    //Frame number of desired page according to TLB
                    frameObtained = TLB[Convert.ToInt32(tlbFoundWhere), 1];

                    //Each row in RAM is a frame number.  Each column is a byte in a frame.
                    valueObtained = RAM[Convert.ToInt32(frameObtained),
                        Convert.ToInt32(ToDecimal(offset))];
                }
                else
                {
                    //Tracking TLB faults.
                    numTlbFaults++;

                    //Checks page table:
                    bool pageTableHit = false; //False until proven true

                    //Will store what page table index page is found in if found there.
                    double pageFoundWhere = 1000;

                    //Going through page table
                    for (int onTblIndex = 0; onTblIndex < PAGE_TBL_SIZE; onTblIndex++)
                    {
                        //Is there a match in the page # column?
                        if (pageTable[onTblIndex, 0] == ToDecimal(pageNum))
                        {
                            pageTableHit = true;
                            pageFoundWhere = onTblIndex;
                        }
                    }

                    //Is there a page table hit?
                    if (pageTableHit)
                    {
                        //Tracking page table hits.
                        numPageTblHits++;

                        frameObtained = Convert.ToInt32(pageTable[Convert.ToInt32(pageFoundWhere), 1]);

                        //Each row in RAM is a frame number.  Each column is a byte in a frame.
                        //There is lots of data converting here: to simplify,
                        //let me say "valueObtained = RAM[pageTable[pageFoundWhere, 1], offset]"
                        valueObtained = RAM[Convert.ToInt32(pageTable[Convert.ToInt32(pageFoundWhere), 1]),
                            Convert.ToInt32(ToDecimal(offset))];

                        //TLB swaps current page in.  In page table, column 1 (in contrast to column 0)
                        //holds the frame number.
                        TLBreplace(ToDecimal(pageNum), pageTable[Convert.ToInt32(pageFoundWhere), 1]);
                    }
                    else
                    {
                        //Tracking page table hits.
                        numPageTblFaults++;

                        //Page table and TLB swap.  PageTblReplace returns frame page is swapped in to.
                        double frameUsed = PageTblReplace(ToDecimal(pageNum));
                        frameObtained = frameUsed; //Just because "frameUsed" needs to be used in an outer loop.
                        TLBreplace(ToDecimal(pageNum), frameUsed);

                        //Now, get the requested information from the place in RAM
                        //it is now in.
                        valueObtained = RAM[Convert.ToInt32(frameUsed),
                                    Convert.ToInt32(ToDecimal(offset))];
                    }
                }

                //Adds current value obtained with tabling enabled
                //and appends it to list of values.
                withTableValues[onLine] = valueObtained.ToString();

                //Updates list of new physical addresses.
                //newPhysicalAddresses[onLine] = (frameObtained * FRAME_SIZE + ToDecimal(offset)).ToString();

                //Progress for each logical address processed.
                operationProgress.Value++;
            }

            //Tracking hit/fault statistics
            double percentagePageTblHits = numPageTblHits / (numPageTblHits + numPageTblFaults) * 100;
            double percentageTlbHits = numTlbHits / (numTlbHits + numTlbFaults) * 100;

            //Reporting hit/fault statistics
            hitFaultRatios.Text = "Statistics: Page Table = " + percentagePageTblHits.ToString() + "% Hits, "
                                + "TLB = " + percentageTlbHits.ToString() + "% Hits";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Writes, to file, tabled and tabeless physical addresses and values. 
            System.IO.File.WriteAllLines("physical_addreses_noPaging.txt", oldPhysicalAddresses);
            System.IO.File.WriteAllLines("noTableValues.txt", noTableValues);
            System.IO.File.WriteAllLines("withTableValues.txt", withTableValues);
            System.IO.File.WriteAllLines("newPhysicalAddresses.txt", newPhysicalAddresses);

            //MessageBox.Show("saving...");
            //Writes all RAM to file
            string[] ramToWrite = new string[256];
            for (int onFrame = 0; onFrame < RAM_SIZE; onFrame++)
            {
                //Goes through all bytes of RAM frame
                for (int onByte = 0; onByte < FRAME_SIZE; onByte++)
                {
                    ramToWrite[onFrame] += " | " + RAM[onFrame, onByte].ToString();
                }
            }
            System.IO.File.WriteAllLines("RAM.txt", ramToWrite);

            //Writes all of TLB to file.
            string[] tlbToWrite = new string[16];
            for (int onPage = 0; onPage < TLB_SIZE; onPage++)
            {
                tlbToWrite[onPage] = "Page #: " + TLB[onPage, 0].ToString() + " | "
                                        + "Frame #: " + TLB[onPage, 1].ToString();
            }
            System.IO.File.WriteAllLines("TLB.txt", tlbToWrite);

            //Writes all of Page table to file.
            string[] pageTblToWrite = new string[128];
            for (int onPage = 0; onPage < PAGE_TBL_SIZE; onPage++)
            {
                try
                {
                    pageTblToWrite[onPage] = "Page #: " + pageTable[onPage, 0].ToString() + " | "
                                        + "Frame #: " + pageTable[onPage, 1].ToString();
                }
                catch (System.IndexOutOfRangeException)
                {
                    //MessageBox.Show(onPage.ToString());
                }
            }
            System.IO.File.WriteAllLines("Page_table.txt", pageTblToWrite);
        }
    }
}
