using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CRAsm
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Assemble()
        {
            Assembler8080 asm = new Assembler8080();
            asm.SourceLines = sourceText.Lines;
            asm.Assemble();
            ListingText.Text = asm.ListingText;

            HexFile hex = new HexFile();
            hex.LoadData(asm.Data, asm.StartAddress, asm.EndAddress);
            hexText.Text = hex.GetText(CRAsm.HexFile.Formats.Display);

            hexFileText.Text = hex.GetText(CRAsm.HexFile.Formats.Intel);

            SetFocus(ListingTab, ListingText);

            File.WriteAllText(ListFile, asm.ListingText);

            if (asm.ErrorCount == 0)
                File.WriteAllText(HexFile, hex.GetText(CRAsm.HexFile.Formats.Intel));
        }

        private void SetFocus(TabPage SelectedTab, TextBox Control)
        {
            tabControl1.SelectedTab = SelectedTab;
            Control.Focus();
            Control.SelectionStart = 0;
            Control.SelectionLength = 0;
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            SetTabWidth(sourceText, 8);
            SetTabWidth(ListingText, 4);
            SetTabWidth(hexText, 4);
            SetTabWidth(hexFileText, 4);

            LoadAsm("TestProgram.asm");
        }

        // set tab stops to a width of 4
        private const int EM_SETTABSTOPS = 0x00CB;

        private string _asmFile;
        private string _listFile;
        private string _hexFile;

        public string AsmFile
        {
            get => _asmFile;
            set { LoadAsm(value); }
        }

        public void LoadAsm(string FileName)
        {
            if (!System.IO.File.Exists(FileName))
                return;

            this._asmFile = FileName;
            ListFile = System.IO.Path.ChangeExtension(FileName, ".LST");
            HexFile = System.IO.Path.ChangeExtension(FileName, ".HEX");

            sourceText.Clear();
            ListingText.Clear();
            hexText.Clear();
            hexFileText.Clear();

            sourceText.Text = System.IO.File.ReadAllText(FileName);
        }

        public string ListFile { get => _listFile; set => _listFile = value; }
        public string HexFile { get => _hexFile; set => _hexFile = value; }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);
        public static void SetTabWidth(TextBox textbox, int tabWidth)
        {
            //Graphics graphics = textbox.CreateGraphics();
            //var characterWidth = (int)graphics.MeasureString("M", textbox.Font).Width;
            //SendMessage(textbox.Handle, EM_SETTABSTOPS, 1,
            //            new int[] { tabWidth * characterWidth });
            SendMessage(textbox.Handle, EM_SETTABSTOPS, 1,
                        new int[] { tabWidth * 4 });
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.FileName = this.AsmFile;
            if (d.ShowDialog() == DialogResult.OK)
            {
                this.AsmFile = d.FileName;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(AsmFile))
                System.IO.File.Move(AsmFile, System.IO.Path.ChangeExtension(AsmFile, ".BAK"));

            File.WriteAllText(AsmFile, sourceText.Text, Encoding.ASCII);
        }

        private void CompileButton_Click(object sender, EventArgs e)
        {
            Assemble();
            ListingText.Focus();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hexFileText.Text))
            {
                MessageBox.Show("Assembly failed. See listing.");
                return;
            }
            SetFocus(hexFileTab, hexFileText);
            Clipboard.SetText(hexFileText.Text);
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
                sourceText.Text = Clipboard.GetText();
            SetFocus(SourceTab, sourceText);
        }

    }
}
