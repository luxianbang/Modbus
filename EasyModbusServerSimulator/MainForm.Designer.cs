namespace EasyModbusServerSimulator
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.离散输入_滚动条 = new System.Windows.Forms.VScrollBar();
            this.离散输入_数据网格视图 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.线圈_滚动条 = new System.Windows.Forms.VScrollBar();
            this.线圈_数据网格视图 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.输入寄存器_滚动条 = new System.Windows.Forms.VScrollBar();
            this.输入寄存器_数据网格视图 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.保持寄存器_滚动条 = new System.Windows.Forms.VScrollBar();
            this.保持寄存器_数据网格视图 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.客户端数 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.协议信息_列表框 = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.显示协议信息_选择框 = new System.Windows.Forms.CheckBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.读线圈选择框 = new System.Windows.Forms.CheckBox();
            this.读离散输入选择框 = new System.Windows.Forms.CheckBox();
            this.读保持寄存器选择框 = new System.Windows.Forms.CheckBox();
            this.读输入寄存器选择框 = new System.Windows.Forms.CheckBox();
            this.写单线圈选择框 = new System.Windows.Forms.CheckBox();
            this.写单寄存器选择框 = new System.Windows.Forms.CheckBox();
            this.写多寄存器选择框 = new System.Windows.Forms.CheckBox();
            this.写多线圈选择框 = new System.Windows.Forms.CheckBox();
            this.btnProperties = new System.Windows.Forms.Button();
            this.ModbusTCPServer绑定源 = new System.Windows.Forms.BindingSource(this.components);
            this.读写多寄存器选择框 = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.性能计数器 = new System.Diagnostics.PerformanceCounter();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.菜单_属性设置 = new System.Windows.Forms.ToolStripMenuItem();
            this.关于 = new System.Windows.Forms.ToolStripMenuItem();
            this.启动_BT = new System.Windows.Forms.Button();
            this.停止_BT = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.离散输入_数据网格视图)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.线圈_数据网格视图)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.输入寄存器_数据网格视图)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.保持寄存器_数据网格视图)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ModbusTCPServer绑定源)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.性能计数器)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(588, 58);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(307, 463);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.离散输入_滚动条);
            this.tabPage1.Controls.Add(this.离散输入_数据网格视图);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(299, 437);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "离散输入";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // 离散输入_滚动条
            // 
            this.离散输入_滚动条.Cursor = System.Windows.Forms.Cursors.Default;
            this.离散输入_滚动条.LargeChange = 20;
            this.离散输入_滚动条.Location = new System.Drawing.Point(264, 2);
            this.离散输入_滚动条.Maximum = 65534;
            this.离散输入_滚动条.Minimum = 1;
            this.离散输入_滚动条.Name = "离散输入_滚动条";
            this.离散输入_滚动条.Padding = new System.Windows.Forms.Padding(1);
            this.离散输入_滚动条.Size = new System.Drawing.Size(21, 437);
            this.离散输入_滚动条.TabIndex = 1;
            this.离散输入_滚动条.Value = 1;
            this.离散输入_滚动条.ValueChanged += new System.EventHandler(this.离散输入_滚动条_值改变);
            // 
            // 离散输入_数据网格视图
            // 
            this.离散输入_数据网格视图.AllowUserToAddRows = false;
            this.离散输入_数据网格视图.AllowUserToDeleteRows = false;
            this.离散输入_数据网格视图.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.离散输入_数据网格视图.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Value});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.离散输入_数据网格视图.DefaultCellStyle = dataGridViewCellStyle8;
            this.离散输入_数据网格视图.Location = new System.Drawing.Point(53, 2);
            this.离散输入_数据网格视图.Name = "离散输入_数据网格视图";
            this.离散输入_数据网格视图.ReadOnly = true;
            this.离散输入_数据网格视图.RowHeadersVisible = false;
            this.离散输入_数据网格视图.ShowEditingIcon = false;
            this.离散输入_数据网格视图.Size = new System.Drawing.Size(204, 437);
            this.离散输入_数据网格视图.TabIndex = 0;
            this.离散输入_数据网格视图.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.离散输入_数据网格视图_单元格双击);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "地址";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Value
            // 
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Value.DefaultCellStyle = dataGridViewCellStyle7;
            this.Value.HeaderText = "值";
            this.Value.Name = "Value";
            this.Value.ReadOnly = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.线圈_滚动条);
            this.tabPage2.Controls.Add(this.线圈_数据网格视图);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(299, 437);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "线圈";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // 线圈_滚动条
            // 
            this.线圈_滚动条.Cursor = System.Windows.Forms.Cursors.Default;
            this.线圈_滚动条.LargeChange = 20;
            this.线圈_滚动条.Location = new System.Drawing.Point(264, 2);
            this.线圈_滚动条.Maximum = 65534;
            this.线圈_滚动条.Minimum = 1;
            this.线圈_滚动条.Name = "线圈_滚动条";
            this.线圈_滚动条.Padding = new System.Windows.Forms.Padding(1);
            this.线圈_滚动条.Size = new System.Drawing.Size(21, 437);
            this.线圈_滚动条.TabIndex = 2;
            this.线圈_滚动条.Value = 1;
            this.线圈_滚动条.ValueChanged += new System.EventHandler(this.线圈_滚动条_值改变);
            // 
            // 线圈_数据网格视图
            // 
            this.线圈_数据网格视图.AllowUserToAddRows = false;
            this.线圈_数据网格视图.AllowUserToDeleteRows = false;
            this.线圈_数据网格视图.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.线圈_数据网格视图.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.线圈_数据网格视图.DefaultCellStyle = dataGridViewCellStyle10;
            this.线圈_数据网格视图.Location = new System.Drawing.Point(53, 2);
            this.线圈_数据网格视图.Name = "线圈_数据网格视图";
            this.线圈_数据网格视图.ReadOnly = true;
            this.线圈_数据网格视图.RowHeadersVisible = false;
            this.线圈_数据网格视图.ShowEditingIcon = false;
            this.线圈_数据网格视图.Size = new System.Drawing.Size(204, 437);
            this.线圈_数据网格视图.TabIndex = 1;
            this.线圈_数据网格视图.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.线圈_数据网格视图_单元格双击);
            this.线圈_数据网格视图.MouseEnter += new System.EventHandler(this.线圈_数据网格视图_鼠标进入);
            this.线圈_数据网格视图.MouseLeave += new System.EventHandler(this.线圈_数据网格视图_鼠标离开);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "地址";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewTextBoxColumn2.HeaderText = "值";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.输入寄存器_滚动条);
            this.tabPage3.Controls.Add(this.输入寄存器_数据网格视图);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(299, 437);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "输入寄存器";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // 输入寄存器_滚动条
            // 
            this.输入寄存器_滚动条.Cursor = System.Windows.Forms.Cursors.Default;
            this.输入寄存器_滚动条.LargeChange = 20;
            this.输入寄存器_滚动条.Location = new System.Drawing.Point(264, 2);
            this.输入寄存器_滚动条.Maximum = 65534;
            this.输入寄存器_滚动条.Minimum = 1;
            this.输入寄存器_滚动条.Name = "输入寄存器_滚动条";
            this.输入寄存器_滚动条.Padding = new System.Windows.Forms.Padding(1);
            this.输入寄存器_滚动条.Size = new System.Drawing.Size(21, 437);
            this.输入寄存器_滚动条.TabIndex = 2;
            this.输入寄存器_滚动条.Value = 1;
            this.输入寄存器_滚动条.ValueChanged += new System.EventHandler(this.输入寄存器_滚动条_值改变);
            // 
            // 输入寄存器_数据网格视图
            // 
            this.输入寄存器_数据网格视图.AllowUserToAddRows = false;
            this.输入寄存器_数据网格视图.AllowUserToDeleteRows = false;
            this.输入寄存器_数据网格视图.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.输入寄存器_数据网格视图.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.输入寄存器_数据网格视图.DefaultCellStyle = dataGridViewCellStyle11;
            this.输入寄存器_数据网格视图.Location = new System.Drawing.Point(53, 2);
            this.输入寄存器_数据网格视图.Name = "输入寄存器_数据网格视图";
            this.输入寄存器_数据网格视图.RowHeadersVisible = false;
            this.输入寄存器_数据网格视图.ShowEditingIcon = false;
            this.输入寄存器_数据网格视图.Size = new System.Drawing.Size(204, 437);
            this.输入寄存器_数据网格视图.TabIndex = 1;
            this.输入寄存器_数据网格视图.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.输入寄存器_数据网格视图_单元格值变更);
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "地址";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "值";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.保持寄存器_滚动条);
            this.tabPage4.Controls.Add(this.保持寄存器_数据网格视图);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(299, 437);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "保持寄存器";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // 保持寄存器_滚动条
            // 
            this.保持寄存器_滚动条.Cursor = System.Windows.Forms.Cursors.Default;
            this.保持寄存器_滚动条.LargeChange = 20;
            this.保持寄存器_滚动条.Location = new System.Drawing.Point(264, 2);
            this.保持寄存器_滚动条.Maximum = 65534;
            this.保持寄存器_滚动条.Minimum = 1;
            this.保持寄存器_滚动条.Name = "保持寄存器_滚动条";
            this.保持寄存器_滚动条.Padding = new System.Windows.Forms.Padding(1);
            this.保持寄存器_滚动条.Size = new System.Drawing.Size(21, 437);
            this.保持寄存器_滚动条.TabIndex = 2;
            this.保持寄存器_滚动条.Value = 1;
            this.保持寄存器_滚动条.ValueChanged += new System.EventHandler(this.保持寄存器_滚动条_值改变);
            // 
            // 保持寄存器_数据网格视图
            // 
            this.保持寄存器_数据网格视图.AllowUserToAddRows = false;
            this.保持寄存器_数据网格视图.AllowUserToDeleteRows = false;
            this.保持寄存器_数据网格视图.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.保持寄存器_数据网格视图.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.保持寄存器_数据网格视图.DefaultCellStyle = dataGridViewCellStyle12;
            this.保持寄存器_数据网格视图.Location = new System.Drawing.Point(53, 2);
            this.保持寄存器_数据网格视图.Name = "保持寄存器_数据网格视图";
            this.保持寄存器_数据网格视图.RowHeadersVisible = false;
            this.保持寄存器_数据网格视图.ShowEditingIcon = false;
            this.保持寄存器_数据网格视图.Size = new System.Drawing.Size(204, 437);
            this.保持寄存器_数据网格视图.TabIndex = 1;
            this.保持寄存器_数据网格视图.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.保持寄存器_数据网格视图_单元格值变更);
            this.保持寄存器_数据网格视图.MouseEnter += new System.EventHandler(this.保持寄存器_数据网格视图_鼠标进入);
            this.保持寄存器_数据网格视图.MouseLeave += new System.EventHandler(this.保持寄存器_数据网格视图_鼠标离开);
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "地址";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "值";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(762, 34);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            65515,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(83, 21);
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(709, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "到地址:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "连接的客户端数";
            // 
            // 客户端数
            // 
            this.客户端数.AutoSize = true;
            this.客户端数.Location = new System.Drawing.Point(110, 106);
            this.客户端数.Name = "客户端数";
            this.客户端数.Size = new System.Drawing.Size(11, 12);
            this.客户端数.TabIndex = 4;
            this.客户端数.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(259, -2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(371, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "...Modbus-TCP Server Listening (Port 8000)...";
            // 
            // 协议信息_列表框
            // 
            this.协议信息_列表框.FormattingEnabled = true;
            this.协议信息_列表框.HorizontalScrollbar = true;
            this.协议信息_列表框.ItemHeight = 12;
            this.协议信息_列表框.Location = new System.Drawing.Point(2, 3);
            this.协议信息_列表框.Name = "协议信息_列表框";
            this.协议信息_列表框.Size = new System.Drawing.Size(571, 160);
            this.协议信息_列表框.TabIndex = 8;
            this.协议信息_列表框.MouseMove += new System.Windows.Forms.MouseEventHandler(this.协议信息_列表框_鼠标移动);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "协议信息";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(15, 309);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(309, 190);
            this.textBox1.TabIndex = 10;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // 显示协议信息_选择框
            // 
            this.显示协议信息_选择框.AutoSize = true;
            this.显示协议信息_选择框.Checked = true;
            this.显示协议信息_选择框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.显示协议信息_选择框.Location = new System.Drawing.Point(428, 119);
            this.显示协议信息_选择框.Name = "显示协议信息_选择框";
            this.显示协议信息_选择框.Size = new System.Drawing.Size(96, 16);
            this.显示协议信息_选择框.TabIndex = 11;
            this.显示协议信息_选择框.Text = "显示协议信息";
            this.显示协议信息_选择框.UseVisualStyleBackColor = true;
            this.显示协议信息_选择框.CheckedChanged += new System.EventHandler(this.显示协议信息_选择框_更改);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(140, 6);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(71, 12);
            this.lblVersion.TabIndex = 12;
            this.lblVersion.Text = "Version 5.6";
            // 
            // 读线圈选择框
            // 
            this.读线圈选择框.AutoSize = true;
            this.读线圈选择框.Checked = true;
            this.读线圈选择框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.读线圈选择框.Location = new System.Drawing.Point(29, 345);
            this.读线圈选择框.Name = "读线圈选择框";
            this.读线圈选择框.Size = new System.Drawing.Size(15, 14);
            this.读线圈选择框.TabIndex = 13;
            this.读线圈选择框.UseVisualStyleBackColor = true;
            this.读线圈选择框.CheckedChanged += new System.EventHandler(this.读线圈选择框_更改);
            // 
            // 读离散输入选择框
            // 
            this.读离散输入选择框.AutoSize = true;
            this.读离散输入选择框.Checked = true;
            this.读离散输入选择框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.读离散输入选择框.Location = new System.Drawing.Point(29, 361);
            this.读离散输入选择框.Name = "读离散输入选择框";
            this.读离散输入选择框.Size = new System.Drawing.Size(15, 14);
            this.读离散输入选择框.TabIndex = 14;
            this.读离散输入选择框.UseVisualStyleBackColor = true;
            this.读离散输入选择框.CheckedChanged += new System.EventHandler(this.读离散输入选择框_更改);
            // 
            // 读保持寄存器选择框
            // 
            this.读保持寄存器选择框.AutoSize = true;
            this.读保持寄存器选择框.Checked = true;
            this.读保持寄存器选择框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.读保持寄存器选择框.Location = new System.Drawing.Point(29, 377);
            this.读保持寄存器选择框.Name = "读保持寄存器选择框";
            this.读保持寄存器选择框.Size = new System.Drawing.Size(15, 14);
            this.读保持寄存器选择框.TabIndex = 15;
            this.读保持寄存器选择框.UseVisualStyleBackColor = true;
            this.读保持寄存器选择框.CheckedChanged += new System.EventHandler(this.读保持寄存器选择框_更改);
            // 
            // 读输入寄存器选择框
            // 
            this.读输入寄存器选择框.AutoSize = true;
            this.读输入寄存器选择框.Checked = true;
            this.读输入寄存器选择框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.读输入寄存器选择框.Location = new System.Drawing.Point(29, 393);
            this.读输入寄存器选择框.Name = "读输入寄存器选择框";
            this.读输入寄存器选择框.Size = new System.Drawing.Size(15, 14);
            this.读输入寄存器选择框.TabIndex = 16;
            this.读输入寄存器选择框.UseVisualStyleBackColor = true;
            this.读输入寄存器选择框.CheckedChanged += new System.EventHandler(this.读输入寄存器选择框_更改);
            // 
            // 写单线圈选择框
            // 
            this.写单线圈选择框.AutoSize = true;
            this.写单线圈选择框.Checked = true;
            this.写单线圈选择框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.写单线圈选择框.Location = new System.Drawing.Point(29, 409);
            this.写单线圈选择框.Name = "写单线圈选择框";
            this.写单线圈选择框.Size = new System.Drawing.Size(15, 14);
            this.写单线圈选择框.TabIndex = 17;
            this.写单线圈选择框.UseVisualStyleBackColor = true;
            this.写单线圈选择框.CheckedChanged += new System.EventHandler(this.写单线圈选择框_更改);
            // 
            // 写单寄存器选择框
            // 
            this.写单寄存器选择框.AutoSize = true;
            this.写单寄存器选择框.Checked = true;
            this.写单寄存器选择框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.写单寄存器选择框.Location = new System.Drawing.Point(29, 425);
            this.写单寄存器选择框.Name = "写单寄存器选择框";
            this.写单寄存器选择框.Size = new System.Drawing.Size(15, 14);
            this.写单寄存器选择框.TabIndex = 18;
            this.写单寄存器选择框.UseVisualStyleBackColor = true;
            this.写单寄存器选择框.CheckedChanged += new System.EventHandler(this.写单寄存器选择框_更改);
            // 
            // 写多寄存器选择框
            // 
            this.写多寄存器选择框.AutoSize = true;
            this.写多寄存器选择框.Checked = true;
            this.写多寄存器选择框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.写多寄存器选择框.Location = new System.Drawing.Point(29, 457);
            this.写多寄存器选择框.Name = "写多寄存器选择框";
            this.写多寄存器选择框.Size = new System.Drawing.Size(15, 14);
            this.写多寄存器选择框.TabIndex = 20;
            this.写多寄存器选择框.UseVisualStyleBackColor = true;
            this.写多寄存器选择框.CheckedChanged += new System.EventHandler(this.写多寄存器选择框_更改);
            // 
            // 写多线圈选择框
            // 
            this.写多线圈选择框.AutoSize = true;
            this.写多线圈选择框.Checked = true;
            this.写多线圈选择框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.写多线圈选择框.Location = new System.Drawing.Point(29, 441);
            this.写多线圈选择框.Name = "写多线圈选择框";
            this.写多线圈选择框.Size = new System.Drawing.Size(15, 14);
            this.写多线圈选择框.TabIndex = 19;
            this.写多线圈选择框.UseVisualStyleBackColor = true;
            this.写多线圈选择框.CheckedChanged += new System.EventHandler(this.写多线圈选择框_更改);
            // 
            // btnProperties
            // 
            this.btnProperties.BackgroundImage = global::Modbus从机模拟.Properties.Resources.configure_2;
            this.btnProperties.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnProperties.Location = new System.Drawing.Point(525, 58);
            this.btnProperties.Name = "btnProperties";
            this.btnProperties.Size = new System.Drawing.Size(54, 48);
            this.btnProperties.TabIndex = 21;
            this.btnProperties.UseVisualStyleBackColor = true;
            this.btnProperties.Click += new System.EventHandler(this.连接设置属性_Click);
            // 
            // 读写多寄存器选择框
            // 
            this.读写多寄存器选择框.AutoSize = true;
            this.读写多寄存器选择框.Checked = true;
            this.读写多寄存器选择框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.读写多寄存器选择框.Location = new System.Drawing.Point(29, 473);
            this.读写多寄存器选择框.Name = "读写多寄存器选择框";
            this.读写多寄存器选择框.Size = new System.Drawing.Size(15, 14);
            this.读写多寄存器选择框.TabIndex = 22;
            this.读写多寄存器选择框.UseVisualStyleBackColor = true;
            this.读写多寄存器选择框.CheckedChanged += new System.EventHandler(this.读写多寄存器选择框_更改);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.协议信息_列表框);
            this.panel1.Location = new System.Drawing.Point(13, 138);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(573, 166);
            this.panel1.TabIndex = 23;
            this.panel1.MouseEnter += new System.EventHandler(this.panel1_MouseEnter);
            this.panel1.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
            // 
            // 性能计数器
            // 
            this.性能计数器.CategoryName = "Processor";
            this.性能计数器.CounterName = "% Processor Time";
            this.性能计数器.InstanceName = "_Total";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.菜单_属性设置,
            this.关于});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(897, 25);
            this.menuStrip1.TabIndex = 24;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 菜单_属性设置
            // 
            this.菜单_属性设置.Name = "菜单_属性设置";
            this.菜单_属性设置.Size = new System.Drawing.Size(68, 21);
            this.菜单_属性设置.Text = "属性设置";
            this.菜单_属性设置.Click += new System.EventHandler(this.连接设置属性_Click);
            // 
            // 关于
            // 
            this.关于.Name = "关于";
            this.关于.Size = new System.Drawing.Size(43, 21);
            this.关于.Text = "Info";
            this.关于.Click += new System.EventHandler(this.关于_Click);
            // 
            // 启动_BT
            // 
            this.启动_BT.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.启动_BT.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.启动_BT.Location = new System.Drawing.Point(263, 43);
            this.启动_BT.Name = "启动_BT";
            this.启动_BT.Size = new System.Drawing.Size(86, 37);
            this.启动_BT.TabIndex = 25;
            this.启动_BT.Text = "启 动";
            this.启动_BT.UseVisualStyleBackColor = false;
            this.启动_BT.Click += new System.EventHandler(this.启动_BT_Click);
            // 
            // 停止_BT
            // 
            this.停止_BT.BackColor = System.Drawing.Color.Brown;
            this.停止_BT.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.停止_BT.Location = new System.Drawing.Point(377, 43);
            this.停止_BT.Name = "停止_BT";
            this.停止_BT.Size = new System.Drawing.Size(86, 37);
            this.停止_BT.TabIndex = 26;
            this.停止_BT.Text = "停 止";
            this.停止_BT.UseVisualStyleBackColor = false;
            this.停止_BT.Click += new System.EventHandler(this.停止_BT_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 526);
            this.Controls.Add(this.停止_BT);
            this.Controls.Add(this.启动_BT);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.读写多寄存器选择框);
            this.Controls.Add(this.btnProperties);
            this.Controls.Add(this.写多寄存器选择框);
            this.Controls.Add(this.写多线圈选择框);
            this.Controls.Add(this.写单寄存器选择框);
            this.Controls.Add(this.写单线圈选择框);
            this.Controls.Add(this.读输入寄存器选择框);
            this.Controls.Add(this.读保持寄存器选择框);
            this.Controls.Add(this.读离散输入选择框);
            this.Controls.Add(this.读线圈选择框);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.显示协议信息_选择框);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.客户端数);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Modbus从机模拟";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.离散输入_数据网格视图)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.线圈_数据网格视图)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.输入寄存器_数据网格视图)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.保持寄存器_数据网格视图)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ModbusTCPServer绑定源)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.性能计数器)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView 离散输入_数据网格视图;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.BindingSource ModbusTCPServer绑定源;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView 线圈_数据网格视图;
        private System.Windows.Forms.DataGridView 输入寄存器_数据网格视图;
        private System.Windows.Forms.DataGridView 保持寄存器_数据网格视图;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label 客户端数;
        private System.Windows.Forms.VScrollBar 离散输入_滚动条;
        private System.Windows.Forms.VScrollBar 线圈_滚动条;
        private System.Windows.Forms.VScrollBar 输入寄存器_滚动条;
        private System.Windows.Forms.VScrollBar 保持寄存器_滚动条;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox 协议信息_列表框;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox 显示协议信息_选择框;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.CheckBox 读线圈选择框;
        private System.Windows.Forms.CheckBox 读离散输入选择框;
        private System.Windows.Forms.CheckBox 读保持寄存器选择框;
        private System.Windows.Forms.CheckBox 读输入寄存器选择框;
        private System.Windows.Forms.CheckBox 写单线圈选择框;
        private System.Windows.Forms.CheckBox 写单寄存器选择框;
        private System.Windows.Forms.CheckBox 写多寄存器选择框;
        private System.Windows.Forms.CheckBox 写多线圈选择框;
        private System.Windows.Forms.Button btnProperties;
        private System.Windows.Forms.CheckBox 读写多寄存器选择框;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Diagnostics.PerformanceCounter 性能计数器;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 菜单_属性设置;
        private System.Windows.Forms.ToolStripMenuItem 关于;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.Button 启动_BT;
        private System.Windows.Forms.Button 停止_BT;
    }
}

