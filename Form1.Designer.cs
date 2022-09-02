namespace NESEmulator
{
    partial class downTenButton
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.debugOutput = new System.Windows.Forms.RichTextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.debugOutput2 = new System.Windows.Forms.RichTextBox();
            this.runButton = new System.Windows.Forms.Button();
            this.romBox = new System.Windows.Forms.TextBox();
            this.pcChecker = new System.Windows.Forms.TextBox();
            this.upOneButton = new System.Windows.Forms.Button();
            this.downOneButton = new System.Windows.Forms.Button();
            this.gotoPRGRom = new System.Windows.Forms.Button();
            this.gotoEnd = new System.Windows.Forms.Button();
            this.downFiveButton = new System.Windows.Forms.Button();
            this.upTenButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.debugViewLabel = new System.Windows.Forms.Label();
            this.gotoSpecified = new System.Windows.Forms.Button();
            this.up25Button = new System.Windows.Forms.Button();
            this.down25Button = new System.Windows.Forms.Button();
            this.down50Button = new System.Windows.Forms.Button();
            this.up50Button = new System.Windows.Forms.Button();
            this.downLineButton = new System.Windows.Forms.Button();
            this.upLineButton = new System.Windows.Forms.Button();
            this.upPageButton = new System.Windows.Forms.Button();
            this.downPageButton = new System.Windows.Forms.Button();
            this.gotoPRGRom2 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.slowExecuteBox = new System.Windows.Forms.CheckBox();
            this.CPUDebug = new System.Windows.Forms.CheckBox();
            this.togglePreview = new System.Windows.Forms.Button();
            this.PCOverrideBox = new System.Windows.Forms.TextBox();
            this.PCOverrideLabel = new System.Windows.Forms.Label();
            this.registersBox = new System.Windows.Forms.RichTextBox();
            this.registersButton = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.debugOutput3 = new System.Windows.Forms.RichTextBox();
            this.DEBUG = new System.Windows.Forms.CheckBox();
            this.DEBUG2 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // debugOutput
            // 
            this.debugOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.debugOutput.DetectUrls = false;
            this.debugOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.debugOutput.Location = new System.Drawing.Point(12, 314);
            this.debugOutput.Name = "debugOutput";
            this.debugOutput.ReadOnly = true;
            this.debugOutput.Size = new System.Drawing.Size(1248, 258);
            this.debugOutput.TabIndex = 0;
            this.debugOutput.Text = "";
            // 
            // startButton
            // 
            this.startButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.startButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startButton.Location = new System.Drawing.Point(12, 12);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(141, 59);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Load";
            this.startButton.UseVisualStyleBackColor = false;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.stopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopButton.Location = new System.Drawing.Point(393, 12);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(147, 59);
            this.stopButton.TabIndex = 2;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // debugOutput2
            // 
            this.debugOutput2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.debugOutput2.DetectUrls = false;
            this.debugOutput2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.debugOutput2.Location = new System.Drawing.Point(12, 103);
            this.debugOutput2.Name = "debugOutput2";
            this.debugOutput2.ReadOnly = true;
            this.debugOutput2.Size = new System.Drawing.Size(528, 205);
            this.debugOutput2.TabIndex = 4;
            this.debugOutput2.Text = "";
            // 
            // runButton
            // 
            this.runButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.runButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runButton.Location = new System.Drawing.Point(159, 12);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(228, 59);
            this.runButton.TabIndex = 5;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = false;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // romBox
            // 
            this.romBox.Location = new System.Drawing.Point(12, 77);
            this.romBox.Name = "romBox";
            this.romBox.Size = new System.Drawing.Size(528, 20);
            this.romBox.TabIndex = 6;
            this.romBox.Text = "nestest";
            // 
            // pcChecker
            // 
            this.pcChecker.Location = new System.Drawing.Point(546, 77);
            this.pcChecker.Name = "pcChecker";
            this.pcChecker.Size = new System.Drawing.Size(123, 20);
            this.pcChecker.TabIndex = 7;
            this.pcChecker.Text = "0";
            // 
            // upOneButton
            // 
            this.upOneButton.BackColor = System.Drawing.Color.LightGreen;
            this.upOneButton.Location = new System.Drawing.Point(689, 103);
            this.upOneButton.Name = "upOneButton";
            this.upOneButton.Size = new System.Drawing.Size(70, 37);
            this.upOneButton.TabIndex = 8;
            this.upOneButton.Text = "Up 1";
            this.upOneButton.UseVisualStyleBackColor = false;
            this.upOneButton.Click += new System.EventHandler(this.upOneButton_Click);
            // 
            // downOneButton
            // 
            this.downOneButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.downOneButton.ForeColor = System.Drawing.Color.Black;
            this.downOneButton.Location = new System.Drawing.Point(689, 191);
            this.downOneButton.Name = "downOneButton";
            this.downOneButton.Size = new System.Drawing.Size(70, 41);
            this.downOneButton.TabIndex = 9;
            this.downOneButton.Text = "Down 1";
            this.downOneButton.UseVisualStyleBackColor = false;
            this.downOneButton.Click += new System.EventHandler(this.downOneButton_Click);
            // 
            // gotoPRGRom
            // 
            this.gotoPRGRom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.gotoPRGRom.Location = new System.Drawing.Point(689, 38);
            this.gotoPRGRom.Name = "gotoPRGRom";
            this.gotoPRGRom.Size = new System.Drawing.Size(67, 59);
            this.gotoPRGRom.TabIndex = 10;
            this.gotoPRGRom.Text = "Goto PRG-ROM";
            this.gotoPRGRom.UseVisualStyleBackColor = false;
            this.gotoPRGRom.Click += new System.EventHandler(this.gotoPRGRom_Click);
            // 
            // gotoEnd
            // 
            this.gotoEnd.BackColor = System.Drawing.Color.LightGreen;
            this.gotoEnd.Location = new System.Drawing.Point(766, 103);
            this.gotoEnd.Name = "gotoEnd";
            this.gotoEnd.Size = new System.Drawing.Size(70, 37);
            this.gotoEnd.TabIndex = 11;
            this.gotoEnd.Text = "Up 5";
            this.gotoEnd.UseVisualStyleBackColor = false;
            this.gotoEnd.Click += new System.EventHandler(this.upFiveButton_Click);
            // 
            // downFiveButton
            // 
            this.downFiveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.downFiveButton.Location = new System.Drawing.Point(766, 191);
            this.downFiveButton.Name = "downFiveButton";
            this.downFiveButton.Size = new System.Drawing.Size(70, 41);
            this.downFiveButton.TabIndex = 12;
            this.downFiveButton.Text = "Down 5";
            this.downFiveButton.UseVisualStyleBackColor = false;
            this.downFiveButton.Click += new System.EventHandler(this.downFiveButton_Click);
            // 
            // upTenButton
            // 
            this.upTenButton.BackColor = System.Drawing.Color.LightGreen;
            this.upTenButton.Location = new System.Drawing.Point(841, 103);
            this.upTenButton.Name = "upTenButton";
            this.upTenButton.Size = new System.Drawing.Size(70, 37);
            this.upTenButton.TabIndex = 13;
            this.upTenButton.Text = "Up 10";
            this.upTenButton.UseVisualStyleBackColor = false;
            this.upTenButton.Click += new System.EventHandler(this.upTenButton_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.button1.Location = new System.Drawing.Point(842, 191);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 41);
            this.button1.TabIndex = 14;
            this.button1.Text = "Down 10";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // debugViewLabel
            // 
            this.debugViewLabel.AutoSize = true;
            this.debugViewLabel.Location = new System.Drawing.Point(546, 16);
            this.debugViewLabel.Name = "debugViewLabel";
            this.debugViewLabel.Size = new System.Drawing.Size(123, 13);
            this.debugViewLabel.TabIndex = 15;
            this.debugViewLabel.Text = "Debug Viewing: Inactive";
            // 
            // gotoSpecified
            // 
            this.gotoSpecified.Location = new System.Drawing.Point(549, 103);
            this.gotoSpecified.Name = "gotoSpecified";
            this.gotoSpecified.Size = new System.Drawing.Size(120, 23);
            this.gotoSpecified.TabIndex = 16;
            this.gotoSpecified.Text = "Goto Specified";
            this.gotoSpecified.UseVisualStyleBackColor = true;
            this.gotoSpecified.Click += new System.EventHandler(this.gotoSpecified_Click);
            // 
            // up25Button
            // 
            this.up25Button.BackColor = System.Drawing.Color.LightGreen;
            this.up25Button.Location = new System.Drawing.Point(689, 146);
            this.up25Button.Name = "up25Button";
            this.up25Button.Size = new System.Drawing.Size(70, 39);
            this.up25Button.TabIndex = 17;
            this.up25Button.Text = "Up 25";
            this.up25Button.UseVisualStyleBackColor = false;
            this.up25Button.Click += new System.EventHandler(this.up25Button_Click);
            // 
            // down25Button
            // 
            this.down25Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.down25Button.Location = new System.Drawing.Point(689, 238);
            this.down25Button.Name = "down25Button";
            this.down25Button.Size = new System.Drawing.Size(70, 44);
            this.down25Button.TabIndex = 18;
            this.down25Button.Text = "Down 25";
            this.down25Button.UseVisualStyleBackColor = false;
            this.down25Button.Click += new System.EventHandler(this.down25Button_Click);
            // 
            // down50Button
            // 
            this.down50Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.down50Button.Location = new System.Drawing.Point(765, 238);
            this.down50Button.Name = "down50Button";
            this.down50Button.Size = new System.Drawing.Size(70, 44);
            this.down50Button.TabIndex = 19;
            this.down50Button.Text = "Down 50";
            this.down50Button.UseVisualStyleBackColor = false;
            this.down50Button.Click += new System.EventHandler(this.down50Button_Click);
            // 
            // up50Button
            // 
            this.up50Button.BackColor = System.Drawing.Color.LightGreen;
            this.up50Button.Location = new System.Drawing.Point(766, 146);
            this.up50Button.Name = "up50Button";
            this.up50Button.Size = new System.Drawing.Size(70, 39);
            this.up50Button.TabIndex = 20;
            this.up50Button.Text = "Up 50";
            this.up50Button.UseVisualStyleBackColor = false;
            this.up50Button.Click += new System.EventHandler(this.up50Button_Click);
            // 
            // downLineButton
            // 
            this.downLineButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.downLineButton.Location = new System.Drawing.Point(841, 238);
            this.downLineButton.Name = "downLineButton";
            this.downLineButton.Size = new System.Drawing.Size(70, 44);
            this.downLineButton.TabIndex = 21;
            this.downLineButton.Text = "Down Line";
            this.downLineButton.UseVisualStyleBackColor = false;
            this.downLineButton.Click += new System.EventHandler(this.down100Button_Click);
            // 
            // upLineButton
            // 
            this.upLineButton.BackColor = System.Drawing.Color.LightGreen;
            this.upLineButton.Location = new System.Drawing.Point(842, 146);
            this.upLineButton.Name = "upLineButton";
            this.upLineButton.Size = new System.Drawing.Size(70, 39);
            this.upLineButton.TabIndex = 22;
            this.upLineButton.Text = "Up Line";
            this.upLineButton.UseVisualStyleBackColor = false;
            this.upLineButton.Click += new System.EventHandler(this.up100Button_Click);
            // 
            // upPageButton
            // 
            this.upPageButton.BackColor = System.Drawing.Color.LightGreen;
            this.upPageButton.Location = new System.Drawing.Point(917, 103);
            this.upPageButton.Name = "upPageButton";
            this.upPageButton.Size = new System.Drawing.Size(70, 82);
            this.upPageButton.TabIndex = 23;
            this.upPageButton.Text = "Up Page";
            this.upPageButton.UseVisualStyleBackColor = false;
            this.upPageButton.Click += new System.EventHandler(this.up200Button_Click);
            // 
            // downPageButton
            // 
            this.downPageButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.downPageButton.Location = new System.Drawing.Point(917, 191);
            this.downPageButton.Name = "downPageButton";
            this.downPageButton.Size = new System.Drawing.Size(70, 91);
            this.downPageButton.TabIndex = 24;
            this.downPageButton.Text = "Down Page";
            this.downPageButton.UseVisualStyleBackColor = false;
            this.downPageButton.Click += new System.EventHandler(this.down200Button_Click);
            // 
            // gotoPRGRom2
            // 
            this.gotoPRGRom2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.gotoPRGRom2.Location = new System.Drawing.Point(762, 38);
            this.gotoPRGRom2.Name = "gotoPRGRom2";
            this.gotoPRGRom2.Size = new System.Drawing.Size(72, 59);
            this.gotoPRGRom2.TabIndex = 25;
            this.gotoPRGRom2.Text = "Goto PRG-ROM 2";
            this.gotoPRGRom2.UseVisualStyleBackColor = false;
            this.gotoPRGRom2.Click += new System.EventHandler(this.gotoPRGRom2_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button2.Location = new System.Drawing.Point(839, 38);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 59);
            this.button2.TabIndex = 26;
            this.button2.Text = "Goto END";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // slowExecuteBox
            // 
            this.slowExecuteBox.AutoSize = true;
            this.slowExecuteBox.Location = new System.Drawing.Point(549, 131);
            this.slowExecuteBox.Name = "slowExecuteBox";
            this.slowExecuteBox.Size = new System.Drawing.Size(91, 17);
            this.slowExecuteBox.TabIndex = 28;
            this.slowExecuteBox.Text = "Slow Execute";
            this.slowExecuteBox.UseVisualStyleBackColor = true;
            // 
            // CPUDebug
            // 
            this.CPUDebug.AutoSize = true;
            this.CPUDebug.Location = new System.Drawing.Point(549, 152);
            this.CPUDebug.Name = "CPUDebug";
            this.CPUDebug.Size = new System.Drawing.Size(83, 17);
            this.CPUDebug.TabIndex = 29;
            this.CPUDebug.Text = "CPU Debug";
            this.CPUDebug.UseVisualStyleBackColor = true;
            // 
            // togglePreview
            // 
            this.togglePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.togglePreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.togglePreview.ForeColor = System.Drawing.Color.Black;
            this.togglePreview.Location = new System.Drawing.Point(993, 285);
            this.togglePreview.Name = "togglePreview";
            this.togglePreview.Size = new System.Drawing.Size(267, 23);
            this.togglePreview.TabIndex = 31;
            this.togglePreview.Text = "PRG -> CHR";
            this.togglePreview.UseVisualStyleBackColor = false;
            this.togglePreview.Click += new System.EventHandler(this.togglePreview_Click);
            // 
            // PCOverrideBox
            // 
            this.PCOverrideBox.Location = new System.Drawing.Point(773, 12);
            this.PCOverrideBox.Name = "PCOverrideBox";
            this.PCOverrideBox.Size = new System.Drawing.Size(63, 20);
            this.PCOverrideBox.TabIndex = 32;
            this.PCOverrideBox.Text = "$C000";
            // 
            // PCOverrideLabel
            // 
            this.PCOverrideLabel.AutoSize = true;
            this.PCOverrideLabel.Location = new System.Drawing.Point(686, 15);
            this.PCOverrideLabel.Name = "PCOverrideLabel";
            this.PCOverrideLabel.Size = new System.Drawing.Size(81, 13);
            this.PCOverrideLabel.TabIndex = 33;
            this.PCOverrideLabel.Text = "PC Init Override";
            // 
            // registersBox
            // 
            this.registersBox.DetectUrls = false;
            this.registersBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.registersBox.Location = new System.Drawing.Point(546, 175);
            this.registersBox.Name = "registersBox";
            this.registersBox.ReadOnly = true;
            this.registersBox.Size = new System.Drawing.Size(134, 78);
            this.registersBox.TabIndex = 34;
            this.registersBox.Text = "";
            // 
            // registersButton
            // 
            this.registersButton.Location = new System.Drawing.Point(546, 259);
            this.registersButton.Name = "registersButton";
            this.registersButton.Size = new System.Drawing.Size(134, 23);
            this.registersButton.TabIndex = 35;
            this.registersButton.Text = "Update";
            this.registersButton.UseVisualStyleBackColor = true;
            this.registersButton.Click += new System.EventHandler(this.registersButton_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button3.Location = new System.Drawing.Point(915, 38);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 59);
            this.button3.TabIndex = 36;
            this.button3.Text = "Goto STACK";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // debugOutput3
            // 
            this.debugOutput3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.debugOutput3.DetectUrls = false;
            this.debugOutput3.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.debugOutput3.Location = new System.Drawing.Point(993, 12);
            this.debugOutput3.Name = "debugOutput3";
            this.debugOutput3.ReadOnly = true;
            this.debugOutput3.Size = new System.Drawing.Size(267, 267);
            this.debugOutput3.TabIndex = 37;
            this.debugOutput3.Text = "";
            // 
            // DEBUG
            // 
            this.DEBUG.AutoSize = true;
            this.DEBUG.Checked = true;
            this.DEBUG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DEBUG.Location = new System.Drawing.Point(546, 288);
            this.DEBUG.Name = "DEBUG";
            this.DEBUG.Size = new System.Drawing.Size(64, 17);
            this.DEBUG.TabIndex = 38;
            this.DEBUG.Text = "DEBUG";
            this.DEBUG.UseVisualStyleBackColor = true;
            // 
            // DEBUG2
            // 
            this.DEBUG2.AutoSize = true;
            this.DEBUG2.Checked = true;
            this.DEBUG2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DEBUG2.Location = new System.Drawing.Point(616, 288);
            this.DEBUG2.Name = "DEBUG2";
            this.DEBUG2.Size = new System.Drawing.Size(73, 17);
            this.DEBUG2.TabIndex = 39;
            this.DEBUG2.Text = "DEBUG 2";
            this.DEBUG2.UseVisualStyleBackColor = true;
            // 
            // downTenButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1272, 584);
            this.Controls.Add(this.DEBUG2);
            this.Controls.Add(this.DEBUG);
            this.Controls.Add(this.debugOutput3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.registersButton);
            this.Controls.Add(this.registersBox);
            this.Controls.Add(this.PCOverrideLabel);
            this.Controls.Add(this.PCOverrideBox);
            this.Controls.Add(this.togglePreview);
            this.Controls.Add(this.CPUDebug);
            this.Controls.Add(this.slowExecuteBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.gotoPRGRom2);
            this.Controls.Add(this.downPageButton);
            this.Controls.Add(this.upPageButton);
            this.Controls.Add(this.upLineButton);
            this.Controls.Add(this.downLineButton);
            this.Controls.Add(this.up50Button);
            this.Controls.Add(this.down50Button);
            this.Controls.Add(this.down25Button);
            this.Controls.Add(this.up25Button);
            this.Controls.Add(this.gotoSpecified);
            this.Controls.Add(this.debugViewLabel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.upTenButton);
            this.Controls.Add(this.downFiveButton);
            this.Controls.Add(this.gotoEnd);
            this.Controls.Add(this.gotoPRGRom);
            this.Controls.Add(this.downOneButton);
            this.Controls.Add(this.upOneButton);
            this.Controls.Add(this.pcChecker);
            this.Controls.Add(this.romBox);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.debugOutput2);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.debugOutput);
            this.Name = "downTenButton";
            this.ShowIcon = false;
            this.Text = "NES Emulator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox debugOutput;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.RichTextBox debugOutput2;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.TextBox romBox;
        private System.Windows.Forms.TextBox pcChecker;
        private System.Windows.Forms.Button upOneButton;
        private System.Windows.Forms.Button downOneButton;
        private System.Windows.Forms.Button gotoPRGRom;
        private System.Windows.Forms.Button gotoEnd;
        private System.Windows.Forms.Button downFiveButton;
        private System.Windows.Forms.Button upTenButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label debugViewLabel;
        private System.Windows.Forms.Button gotoSpecified;
        private System.Windows.Forms.Button up25Button;
        private System.Windows.Forms.Button down25Button;
        private System.Windows.Forms.Button down50Button;
        private System.Windows.Forms.Button up50Button;
        private System.Windows.Forms.Button downLineButton;
        private System.Windows.Forms.Button upLineButton;
        private System.Windows.Forms.Button upPageButton;
        private System.Windows.Forms.Button downPageButton;
        private System.Windows.Forms.Button gotoPRGRom2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox slowExecuteBox;
        private System.Windows.Forms.CheckBox CPUDebug;
        private System.Windows.Forms.Button togglePreview;
        private System.Windows.Forms.TextBox PCOverrideBox;
        private System.Windows.Forms.Label PCOverrideLabel;
        private System.Windows.Forms.RichTextBox registersBox;
        private System.Windows.Forms.Button registersButton;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RichTextBox debugOutput3;
        private System.Windows.Forms.CheckBox DEBUG;
        private System.Windows.Forms.CheckBox DEBUG2;
    }
}

