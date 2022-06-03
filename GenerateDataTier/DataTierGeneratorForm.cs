using System;
using System.Windows.Forms;
using ICEBG.GeneratorUtilities;

namespace ICEBG.GenerateDataTier
{
    public partial class DataTierGeneratorForm : Form
    {
        private bool m_Complete;

        public DataTierGeneratorForm()
        {
            InitializeComponent();

            GenerateDataTier.databaseCounted += Generator_DatabaseCounted;
            GenerateDataTier.tableCounted += Generator_TableCounted;
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!m_Complete)
                {
                    string connectionString;

                    generateButton.Enabled = false;
                    progressBar.Value = 0;
                    string outputDirectory = "C:\\Solutions\\OHI\\ICEBG\\GeneratedFiles";

                    // Generate the SQL and C# code
                    connectionString =
                        "Server=localhost; Database=ICEBG; Integrated Security=SSPI;";
                    GenerateDataTier.GenerateDataTierElements(
                        connectionString,
                        "ICEBG",
                        outputDirectory,
                        true);

                    // Inform the user we're done
                    progressBar.Value = progressBar.Maximum;
                    m_Complete = true;
                    generateButton.Text = "Exit";
                    generateButton.Enabled = true;
                    labelStatus.Text = "C# classes and stored procedures generated successfully.";
                }
                else
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Generator_DatabaseCounted(object sender, CountEventArgs e)
        {
            progressBar.Maximum = e.Count;
        }

        private void Generator_TableCounted(object sender, CountEventArgs e)
        {
            progressBar.Value = e.Count;
        }
    }
}
