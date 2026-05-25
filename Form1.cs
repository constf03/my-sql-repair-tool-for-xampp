namespace MySQLRepairTool
{
    public partial class Form1 : Form
    {
        private RepairService _repairService = new RepairService();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fDiag = new FolderBrowserDialog();

            if (fDiag.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fDiag.SelectedPath;
                button2.Enabled = true;
                label2.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text;
            _repairService.Repair(path);

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
