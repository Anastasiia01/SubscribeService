using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/* TODO: The above example had hardcoded the stock symbol and 
 * the trigger price to be 120. Modify the above implementation so 
 * that the client’s request in the subscribe call determines the symbol 
 * and the trigger price, for the service to be able make a callback 
 * to the client for this condition only.*/

namespace PubSubClient
{
    public partial class Form1 : Form
    {
        STKClient stkC = new STKClient();
        List<string> symbols = new List<string>();
        int myId = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTestCallback_Click(object sender, EventArgs e)
        {
            try
            {
                CBClient cbc = new CBClient();
                cbc.CallLongCompute(int.Parse(txtA.Text), int.Parse(txtB.Text), txtClientID.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnChangePrice_Click(object sender, EventArgs e)
        {
            PS.PriceChangeClient pcc = new PS.PriceChangeClient();
            pcc.ChangeStockPrice(txtSymbol.Text, double.Parse(txtNewPrice.Text));
        }

        private void btnSubscribe_Click(object sender, EventArgs e)
        {
            try
            {
                stkC.SubscribeToPriceChange(txtSymbol.Text, double.Parse(txtNewPrice.Text), myId);
                symbols.Add(txtSymbol.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUnsubscribe_Click(object sender, EventArgs e)
        {
            try
            {
                stkC.UnSubscribeToPriceChange(txtSymbol.Text);
                symbols.Remove(txtSymbol.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach(string sym in symbols)
                    stkC.UnSubscribeToPriceChange(sym);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myId = new Random((int)DateTime.Now.Ticks).Next();
            this.Text = myId.ToString();
        }
    }
}
