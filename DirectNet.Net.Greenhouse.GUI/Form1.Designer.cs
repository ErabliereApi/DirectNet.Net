namespace DirectNet.Net.Greenhouse.GUI;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        statusStrip1 = new StatusStrip();
        toolStripStatusLabel1 = new ToolStripStatusLabel();
        toolStripStatusLabel2 = new ToolStripStatusLabel();
        toolStripStatusLabel3 = new ToolStripStatusLabel();
        toolStripStatusLabel4 = new ToolStripStatusLabel();
        toolStripStatusLabel5 = new ToolStripStatusLabel();
        toolStripStatusLabelError = new ToolStripStatusLabel();
        toolStrip1 = new ToolStrip();
        toolStripComboBox1 = new ToolStripComboBox();
        toolStripResetDriverButton = new ToolStripButton();
        groupBox1 = new GroupBox();
        groupBox2 = new GroupBox();
        groupBox3 = new GroupBox();
        groupBox4 = new GroupBox();
        groupBox5 = new GroupBox();
        groupBox6 = new GroupBox();
        groupBox7 = new GroupBox();
        groupBox8 = new GroupBox();
        groupBox9 = new GroupBox();
        groupBox10 = new GroupBox();
        groupBox11 = new GroupBox();
        groupBox12 = new GroupBox();
        groupBox13 = new GroupBox();
        groupBox14 = new GroupBox();
        groupBox15 = new GroupBox();
        groupBox16 = new GroupBox();
        statusStrip1.SuspendLayout();
        toolStrip1.SuspendLayout();
        groupBox1.SuspendLayout();
        groupBox2.SuspendLayout();
        SuspendLayout();
        // 
        // statusStrip1
        // 
        statusStrip1.ImageScalingSize = new Size(20, 20);
        statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2, toolStripStatusLabel3, toolStripStatusLabel4, toolStripStatusLabel5, toolStripStatusLabelError });
        statusStrip1.Location = new Point(0, 665);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Padding = new Padding(1, 0, 16, 0);
        statusStrip1.Size = new Size(727, 26);
        statusStrip1.TabIndex = 1;
        statusStrip1.Text = "statusStrip1";
        // 
        // toolStripStatusLabel1
        // 
        toolStripStatusLabel1.Name = "toolStripStatusLabel1";
        toolStripStatusLabel1.Size = new Size(106, 20);
        toolStripStatusLabel1.Text = "Scan Time: n/a";
        toolStripStatusLabel1.Click += ToolStripStatusLabel1_Click;
        // 
        // toolStripStatusLabel2
        // 
        toolStripStatusLabel2.Name = "toolStripStatusLabel2";
        toolStripStatusLabel2.Size = new Size(86, 20);
        toolStripStatusLabel2.Text = "State: Close";
        toolStripStatusLabel2.Click += ToolStripStatusLabel2_Click;
        // 
        // toolStripStatusLabel3
        // 
        toolStripStatusLabel3.Name = "toolStripStatusLabel3";
        toolStripStatusLabel3.Size = new Size(0, 20);
        // 
        // toolStripStatusLabel4
        // 
        toolStripStatusLabel4.Name = "toolStripStatusLabel4";
        toolStripStatusLabel4.Size = new Size(91, 20);
        toolStripStatusLabel4.Text = "Enquery: n/a";
        // 
        // toolStripStatusLabel5
        // 
        toolStripStatusLabel5.Name = "toolStripStatusLabel5";
        toolStripStatusLabel5.Size = new Size(93, 20);
        toolStripStatusLabel5.Text = "ErabliereAPI:";
        // 
        // toolStripStatusLabelError
        // 
        toolStripStatusLabelError.Name = "toolStripStatusLabelError";
        toolStripStatusLabelError.Size = new Size(0, 20);
        // 
        // toolStrip1
        // 
        toolStrip1.ImageScalingSize = new Size(20, 20);
        toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripComboBox1, toolStripResetDriverButton });
        toolStrip1.Location = new Point(0, 0);
        toolStrip1.Name = "toolStrip1";
        toolStrip1.Size = new Size(727, 28);
        toolStrip1.TabIndex = 2;
        toolStrip1.Text = "toolStrip1";
        // 
        // toolStripComboBox1
        // 
        toolStripComboBox1.Name = "toolStripComboBox1";
        toolStripComboBox1.Size = new Size(138, 28);
        // 
        // toolStripResetDriverButton
        // 
        toolStripResetDriverButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        toolStripResetDriverButton.Image = (Image)resources.GetObject("toolStripResetDriverButton.Image");
        toolStripResetDriverButton.ImageTransparentColor = Color.Magenta;
        toolStripResetDriverButton.Name = "toolStripResetDriverButton";
        toolStripResetDriverButton.Size = new Size(29, 25);
        toolStripResetDriverButton.Text = "Reset Driver";
        toolStripResetDriverButton.Click += ToolStripResetDriverButton_Click;
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(groupBox14);
        groupBox1.Controls.Add(groupBox13);
        groupBox1.Controls.Add(groupBox12);
        groupBox1.Controls.Add(groupBox11);
        groupBox1.Controls.Add(groupBox10);
        groupBox1.Controls.Add(groupBox9);
        groupBox1.Controls.Add(groupBox8);
        groupBox1.Controls.Add(groupBox7);
        groupBox1.Controls.Add(groupBox6);
        groupBox1.Controls.Add(groupBox5);
        groupBox1.Controls.Add(groupBox4);
        groupBox1.Controls.Add(groupBox3);
        groupBox1.Location = new Point(12, 31);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(703, 273);
        groupBox1.TabIndex = 3;
        groupBox1.TabStop = false;
        groupBox1.Text = "groupBox1";
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(groupBox16);
        groupBox2.Controls.Add(groupBox15);
        groupBox2.Location = new Point(12, 310);
        groupBox2.Name = "groupBox2";
        groupBox2.Size = new Size(703, 332);
        groupBox2.TabIndex = 4;
        groupBox2.TabStop = false;
        groupBox2.Text = "groupBox2";
        // 
        // groupBox3
        // 
        groupBox3.Location = new Point(6, 26);
        groupBox3.Name = "groupBox3";
        groupBox3.Size = new Size(108, 108);
        groupBox3.TabIndex = 0;
        groupBox3.TabStop = false;
        groupBox3.Text = "groupBox3";
        // 
        // groupBox4
        // 
        groupBox4.Location = new Point(120, 26);
        groupBox4.Name = "groupBox4";
        groupBox4.Size = new Size(111, 108);
        groupBox4.TabIndex = 1;
        groupBox4.TabStop = false;
        groupBox4.Text = "groupBox4";
        // 
        // groupBox5
        // 
        groupBox5.Location = new Point(237, 26);
        groupBox5.Name = "groupBox5";
        groupBox5.Size = new Size(123, 108);
        groupBox5.TabIndex = 1;
        groupBox5.TabStop = false;
        groupBox5.Text = "groupBox5";
        // 
        // groupBox6
        // 
        groupBox6.Location = new Point(366, 26);
        groupBox6.Name = "groupBox6";
        groupBox6.Size = new Size(112, 108);
        groupBox6.TabIndex = 1;
        groupBox6.TabStop = false;
        groupBox6.Text = "groupBox6";
        // 
        // groupBox7
        // 
        groupBox7.Location = new Point(484, 26);
        groupBox7.Name = "groupBox7";
        groupBox7.Size = new Size(106, 108);
        groupBox7.TabIndex = 1;
        groupBox7.TabStop = false;
        groupBox7.Text = "groupBox7";
        // 
        // groupBox8
        // 
        groupBox8.Location = new Point(596, 26);
        groupBox8.Name = "groupBox8";
        groupBox8.Size = new Size(106, 108);
        groupBox8.TabIndex = 2;
        groupBox8.TabStop = false;
        groupBox8.Text = "groupBox8";
        // 
        // groupBox9
        // 
        groupBox9.Location = new Point(6, 140);
        groupBox9.Name = "groupBox9";
        groupBox9.Size = new Size(108, 108);
        groupBox9.TabIndex = 1;
        groupBox9.TabStop = false;
        groupBox9.Text = "groupBox9";
        // 
        // groupBox10
        // 
        groupBox10.Location = new Point(120, 140);
        groupBox10.Name = "groupBox10";
        groupBox10.Size = new Size(108, 108);
        groupBox10.TabIndex = 1;
        groupBox10.TabStop = false;
        groupBox10.Text = "groupBox10";
        // 
        // groupBox11
        // 
        groupBox11.Location = new Point(237, 140);
        groupBox11.Name = "groupBox11";
        groupBox11.Size = new Size(108, 108);
        groupBox11.TabIndex = 1;
        groupBox11.TabStop = false;
        groupBox11.Text = "groupBox11";
        // 
        // groupBox12
        // 
        groupBox12.Location = new Point(366, 140);
        groupBox12.Name = "groupBox12";
        groupBox12.Size = new Size(108, 108);
        groupBox12.TabIndex = 1;
        groupBox12.TabStop = false;
        groupBox12.Text = "groupBox12";
        // 
        // groupBox13
        // 
        groupBox13.Location = new Point(484, 140);
        groupBox13.Name = "groupBox13";
        groupBox13.Size = new Size(108, 108);
        groupBox13.TabIndex = 1;
        groupBox13.TabStop = false;
        groupBox13.Text = "groupBox13";
        // 
        // groupBox14
        // 
        groupBox14.Location = new Point(598, 140);
        groupBox14.Name = "groupBox14";
        groupBox14.Size = new Size(108, 108);
        groupBox14.TabIndex = 1;
        groupBox14.TabStop = false;
        groupBox14.Text = "groupBox14";
        // 
        // groupBox15
        // 
        groupBox15.Location = new Point(6, 26);
        groupBox15.Name = "groupBox15";
        groupBox15.Size = new Size(339, 289);
        groupBox15.TabIndex = 0;
        groupBox15.TabStop = false;
        groupBox15.Text = "groupBox15";
        // 
        // groupBox16
        // 
        groupBox16.Location = new Point(361, 26);
        groupBox16.Name = "groupBox16";
        groupBox16.Size = new Size(336, 289);
        groupBox16.TabIndex = 1;
        groupBox16.TabStop = false;
        groupBox16.Text = "groupBox16";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(727, 691);
        Controls.Add(groupBox2);
        Controls.Add(groupBox1);
        Controls.Add(toolStrip1);
        Controls.Add(statusStrip1);
        Margin = new Padding(3, 4, 3, 4);
        Name = "Form1";
        Text = "DirectNet.Net.Greenhouse.GUI";
        FormClosing += Form1_FormClosing;
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        toolStrip1.ResumeLayout(false);
        toolStrip1.PerformLayout();
        groupBox1.ResumeLayout(false);
        groupBox2.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel toolStripStatusLabel1;
    private ToolStrip toolStrip1;
    private ToolStripComboBox toolStripComboBox1;
    private ToolStripStatusLabel toolStripStatusLabel2;
    private ToolStripStatusLabel toolStripStatusLabel3;
    private ToolStripStatusLabel toolStripStatusLabel4;
    private ToolStripStatusLabel toolStripStatusLabel5;
    private ToolStripStatusLabel toolStripStatusLabelError;
    private ToolStripButton toolStripResetDriverButton;
    private GroupBox groupBox1;
    private GroupBox groupBox2;
    private GroupBox groupBox14;
    private GroupBox groupBox13;
    private GroupBox groupBox12;
    private GroupBox groupBox11;
    private GroupBox groupBox10;
    private GroupBox groupBox9;
    private GroupBox groupBox8;
    private GroupBox groupBox7;
    private GroupBox groupBox6;
    private GroupBox groupBox5;
    private GroupBox groupBox4;
    private GroupBox groupBox3;
    private GroupBox groupBox16;
    private GroupBox groupBox15;
}
