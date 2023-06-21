using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace FaceMaker
{
    partial class forms
    {
        private string ImageUrl;

        public void Awake()
        {
            getpicturebtn.Click += Getpicturebtn_Click;
            compilebtn.Click += Compilebtn_Click;
        }

        private void Compilebtn_Click(object sender, System.EventArgs e)
        {
            if (!File.Exists(ImageUrl))
            {
                MessageBox.Show("Please attach a photo.");
                return;
            }

            string tempPath = Directory.GetCurrentDirectory() + "/temp";

            GorillaFaces.Models.Package package = new GorillaFaces.Models.Package(titlefield.Text, authorfield.Text);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(package);

            Directory.CreateDirectory(tempPath);
            File.WriteAllText(tempPath + "/package.json", json);
            File.Copy(ImageUrl, tempPath + "/image.png");

            ZipFile.CreateFromDirectory(tempPath, Directory.GetCurrentDirectory() + $"/{titlefield.Text}.Face");

            Directory.Delete(tempPath, true);
            MessageBox.Show("Finished compiling!");
        }

        private void Getpicturebtn_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png) | *.png";
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Title = "Select an Image File";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImageUrl = openFileDialog.FileName;
            }

            previewImage.ImageLocation = ImageUrl;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(forms));
            this.compilebtn = new System.Windows.Forms.Button();
            this.titlelabel = new System.Windows.Forms.Label();
            this.titlefield = new System.Windows.Forms.TextBox();
            this.authorlabel = new System.Windows.Forms.Label();
            this.authorfield = new System.Windows.Forms.TextBox();
            this.getpicturebtn = new System.Windows.Forms.Button();
            this.previewImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.previewImage)).BeginInit();
            this.SuspendLayout();
            // 
            // compilebtn
            // 
            this.compilebtn.BackColor = System.Drawing.Color.Black;
            this.compilebtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compilebtn.ForeColor = System.Drawing.SystemColors.Control;
            this.compilebtn.Location = new System.Drawing.Point(12, 289);
            this.compilebtn.Name = "compilebtn";
            this.compilebtn.Size = new System.Drawing.Size(210, 60);
            this.compilebtn.TabIndex = 0;
            this.compilebtn.Text = "Compile";
            this.compilebtn.UseVisualStyleBackColor = false;
            // 
            // titlelabel
            // 
            this.titlelabel.AutoSize = true;
            this.titlelabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titlelabel.Location = new System.Drawing.Point(7, 31);
            this.titlelabel.Name = "titlelabel";
            this.titlelabel.Size = new System.Drawing.Size(68, 29);
            this.titlelabel.TabIndex = 1;
            this.titlelabel.Text = "Title:";
            // 
            // titlefield
            // 
            this.titlefield.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.titlefield.Location = new System.Drawing.Point(88, 40);
            this.titlefield.Name = "titlefield";
            this.titlefield.Size = new System.Drawing.Size(125, 20);
            this.titlefield.TabIndex = 2;
            this.titlefield.Text = "My Title";
            // 
            // authorlabel
            // 
            this.authorlabel.AutoSize = true;
            this.authorlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.authorlabel.Location = new System.Drawing.Point(7, 106);
            this.authorlabel.Name = "authorlabel";
            this.authorlabel.Size = new System.Drawing.Size(68, 22);
            this.authorlabel.TabIndex = 3;
            this.authorlabel.Text = "Author:";
            // 
            // authorfield
            // 
            this.authorfield.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.authorfield.Location = new System.Drawing.Point(88, 110);
            this.authorfield.Name = "authorfield";
            this.authorfield.Size = new System.Drawing.Size(125, 20);
            this.authorfield.TabIndex = 4;
            this.authorfield.Text = "My Author";
            // 
            // getpicturebtn
            // 
            this.getpicturebtn.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.getpicturebtn.Location = new System.Drawing.Point(118, 262);
            this.getpicturebtn.Name = "getpicturebtn";
            this.getpicturebtn.Size = new System.Drawing.Size(104, 21);
            this.getpicturebtn.TabIndex = 5;
            this.getpicturebtn.Text = "Attach Photo";
            this.getpicturebtn.UseVisualStyleBackColor = false;
            // 
            // previewImage
            // 
            this.previewImage.Image = global::FakeMaker.Properties.Resources.gorillachestface;
            this.previewImage.Location = new System.Drawing.Point(11, 189);
            this.previewImage.Name = "previewImage";
            this.previewImage.Size = new System.Drawing.Size(100, 94);
            this.previewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.previewImage.TabIndex = 6;
            this.previewImage.TabStop = false;
            // 
            // forms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.ClientSize = new System.Drawing.Size(234, 361);
            this.Controls.Add(this.previewImage);
            this.Controls.Add(this.getpicturebtn);
            this.Controls.Add(this.authorfield);
            this.Controls.Add(this.authorlabel);
            this.Controls.Add(this.titlefield);
            this.Controls.Add(this.titlelabel);
            this.Controls.Add(this.compilebtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(250, 400);
            this.MinimumSize = new System.Drawing.Size(250, 400);
            this.Name = "forms";
            this.Text = "Face Maker";
            ((System.ComponentModel.ISupportInitialize)(this.previewImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button compilebtn;
        private System.Windows.Forms.Label titlelabel;
        private System.Windows.Forms.TextBox titlefield;
        private System.Windows.Forms.Label authorlabel;
        private System.Windows.Forms.TextBox authorfield;
        private System.Windows.Forms.Button getpicturebtn;
        private PictureBox previewImage;
    }
}

