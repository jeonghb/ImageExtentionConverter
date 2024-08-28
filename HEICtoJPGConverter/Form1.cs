using ImageMagick;
using System;
using System.ComponentModel;
using System.Diagnostics.PerformanceData;
using System.Reflection;

namespace HEICtoJPGConverter
{
    public partial class Form1 : Form
    {
        enum ConvertType
        {
            [Description("HEIC -> JPG")]
            HEICtoJPG,
        }

        public Form1()
        {
            InitializeComponent();

            foreach (ConvertType type in Enum.GetValues(typeof(ConvertType)))
            {
                string description = GetEnumDescription(type);
                if (description != "") CbConvertType.Items.Add(new KeyValuePair<ConvertType, string>(type, description));
            }

            CbConvertType.DisplayMember = "Value";
            CbConvertType.ValueMember = "Key";

            if (CbConvertType.Items.Count > 0)
                CbConvertType.SelectedIndex = 0;
        }

        private string GetEnumDescription(Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());
            if (fi == null) return "";

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        private void BtnDirectorySelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog directory = new()
            {
                ShowNewFolderButton = false
            };

            if (directory.ShowDialog() != DialogResult.OK) return;

            TbxDirectoryPath.Text = directory.SelectedPath;
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            if (TbxDirectoryPath.Text.Trim().Length == 0)
            {
                MessageBox.Show("���丮�� ���õ��� �ʾҾ��.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                BtnConvert.Enabled = false;
                BtnConvert.Text = "��ȯ ��";

                switch (((KeyValuePair<ConvertType, string>)(CbConvertType.SelectedItem)).Key)
                {
                    case ConvertType.HEICtoJPG:
                        string[] allfiles = Directory.GetFiles(TbxDirectoryPath.Text, "*.heic");

                        foreach (var file in allfiles)
                        {
                            FileInfo info = new FileInfo(file);
                            using (MagickImage image = new MagickImage(info.FullName))
                            {
                                // Save frame as jpg
                                image.Write($"{TbxDirectoryPath.Text}\\{info.Name.Replace(".heic", "")}.jpg");
                            }
                        }
                        break;
                }

                MessageBox.Show("��ȯ �Ϸ�!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                BtnConvert.Enabled = true;
                BtnConvert.Text = "��ȯ";
            }
        }

        private void BtnSelectTypeDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("������ Ÿ���� ���ϵ��� ���丮���� ���� �����Ұǰ���?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            switch (((KeyValuePair<ConvertType, string>)(CbConvertType.SelectedItem)).Key)
            {
                case ConvertType.HEICtoJPG:
                    string[] allfiles = Directory.GetFiles(TbxDirectoryPath.Text, "*.heic");

                    foreach (var file in allfiles)
                    {
                        File.Delete(file);
                    }
                    break;
            }

            MessageBox.Show("���� �Ϸ�!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
