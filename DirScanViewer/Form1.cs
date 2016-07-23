using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirScanViewer
{
    public partial class Form1 : Form
    {
        string ResSrc = "";
        struct Record
        {
            public bool IsDir;
            public int Depth;
            public string WTime;
            public string Name;
            public string Size;
        }
        Record[] Records;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                FileName = "FileScanResult",
                Multiselect = false
            };

            if (Environment.GetCommandLineArgs().Count() > 1)
                ResSrc = Environment.GetCommandLineArgs()[1];
            else if (opf.ShowDialog() == DialogResult.OK)
            {
                ResSrc = opf.FileName;
                this.Text = "DirScanViewer - " + opf.SafeFileName;
                LoadContent();
            }
            else
            {
                Application.Exit();
            }
            opf.Dispose();
        }

        private void LoadContent()
        {
            StreamReader Src = new StreamReader(ResSrc);
            String tmp = Src.ReadLine();
            if (!tmp.StartsWith("DirScannerResult"))
            {
                MessageBox.Show("File not supported.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            label1.Text = tmp.Substring(17) + "\n" + Src.ReadLine();

            List<Record> Recs = new List<Record>();
            treeView1.Nodes.Clear();
            TreeNode ScanRoot = new TreeNode ();
            Recs.Clear();

            while (!Src.EndOfStream)
            {
                tmp = Src.ReadLine();
                if (tmp.Count() > 2)
                    Recs.Add(ResolveRecord(tmp));
            }
            Records = Recs.ToArray();
            int Index = 0;
            BuildTree(ref Records, ScanRoot, ref Index, 0);
            TreeNode[] FirstLayer = new TreeNode[ScanRoot.GetNodeCount(false)];
            ScanRoot.Nodes.CopyTo(FirstLayer,0);
            treeView1.Nodes.AddRange(FirstLayer);

            Src.Close();
        }

        private Record ResolveRecord(string LineRecord)
        {
            Record Rec = new Record();
            Rec.IsDir = LineRecord.EndsWith(" >");
            LineRecord = LineRecord.Substring(1);
            Rec.Depth = LineRecord.IndexOf(' ');
            LineRecord = LineRecord.Substring(Rec.Depth + 1);
            Rec.WTime = LineRecord.Substring(1, LineRecord.IndexOf(']') - 1);
            LineRecord = LineRecord.Remove(0, LineRecord.IndexOf(']') + 2);
            if (Rec.IsDir)
            {
                Rec.Name = LineRecord.Remove(LineRecord.Length - 2, 2);
                Rec.Size = "";
            }
            else
            {
                Rec.Name = LineRecord.Substring(LineRecord.IndexOf(']') + 2);
                Rec.Size = LineRecord.Substring(1, LineRecord.IndexOf(']') - 1);
            }
            return Rec;
        }

        private void BuildTree(ref Record[] Records, TreeNode Parent, ref int Index, int Depth)
        {
            while (Index <= Records.GetUpperBound(0))
            {
                if (Records[Index].Depth == Depth)
                {
                    string info;
                    Color Bg;
                    if (Records[Index].IsDir)
                    {
                        info = Records[Index].WTime + " " + "Directory";
                        Bg = Color.LightBlue;
                    }
                    else
                    {
                        info = Records[Index].WTime + " " + Records[Index].Size;
                        Bg = Color.LightGreen;
                    }
                    Parent.Nodes.Add(new TreeNode()
                    {
                        BackColor = Bg,
                        Tag = Records[Index],
                        Text = Records[Index].Name,
                        ToolTipText = info,
                    });
                }
                else if (Records[Index].Depth > Depth)
                    BuildTree(ref Records, Parent.LastNode, ref Index, Depth + 1);
                else if (Records[Index].Depth < Depth)
                {
                    Index--;
                    return;
                }
                Index++;
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            label2.Text = treeView1.SelectedNode.ToolTipText;
        }
    }
}
