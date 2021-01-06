using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SummaryDataGridView
{
    public partial class DataGridViewSummary : DataGridView, ISupportInitialize
    {

        #region Browsable properties

        /// <summary>
        /// If true a row header at the left side 
        /// of the summaryboxes is displayed.
        /// </summary>
        private bool displaySumRowHeader=true;
        [Browsable(true), Category("Summary") , DefaultValue(true)]
        public bool DisplaySumRowHeader
        {
            get { return displaySumRowHeader; }
            set { displaySumRowHeader = value; }
        }

   
        private int headerFontSize=10;
        [Browsable(true), Category("Summary"), DefaultValue(10)]
        public int HeaderFontSize
        {
            get { return headerFontSize; }
            set { headerFontSize = value; }
        }

        private int cellFontSize = 10;
        [Browsable(true), Category("Summary"), DefaultValue(10)]
        public int CellerFontSize
        {
            get { return cellFontSize; }
            set { cellFontSize = value; }
        }
        /// <summary>
        /// Text displayed in the row header
        /// of the summary row.
        /// </summary>
        private string sumRowHeaderText;
        [Browsable(true), Category("Summary")]
        public string SumRowHeaderText
        {
            get { return sumRowHeaderText; }
            set { sumRowHeaderText = value; }
        }

        /// <summary>
        /// Text displayed in the row header
        /// of the summary row.
        /// </summary>
        private bool sumRowHeaderTextBold=true;
        [Browsable(true), Category("Summary"),DefaultValue(true)]
        public bool SumRowHeaderTextBold
        {
            get { return sumRowHeaderTextBold; }
            set { sumRowHeaderTextBold = value; }
        }

        private bool hasCheckBoxColumn;
        [Browsable(true), Category("Summary")]
        public bool HasCheckBoxColumn
        {
            get { return hasCheckBoxColumn; }
            set { hasCheckBoxColumn = value; }
        }

        /// <summary>
        /// Add columns to sum up in text form
        /// </summary>
        private string[] summaryColumns;
        [Browsable(true), Category("Summary")]
        public string[] SummaryColumns
        {
            get { return summaryColumns; }
            set { summaryColumns = value; }
        }

        /// <summary>
        /// Display the summary Row
        /// </summary>
        private bool summaryRowVisible;
        [Browsable(true), Category("Summary")]
        public bool SummaryRowVisible
        {
            get { return summaryRowVisible; }
            set
            {
                summaryRowVisible = value;
                if (summaryControl != null && spacePanel != null)
                {
                    summaryControl.Visible = value;
                    spacePanel.Visible = value;
                }
            }
        }

        private int summaryRowSpace = 0;
        [Browsable(true), Category("Summary")]
        public int SummaryRowSpace
        {
            get { return summaryRowSpace; }
            set { summaryRowSpace = value; }
        }

        private string formatString;// = "F02";
        [Browsable(true), Category("Summary"), DefaultValue("F02")]
        public string FormatString
        {
            get { return formatString; }
            set { formatString = value; }
        }

        [Browsable(true), Category("Summary")]
        public Color SummaryRowBackColor
        {
            get { return this.summaryControl.SummaryRowBackColor; }
            set { summaryControl.SummaryRowBackColor = value; }
        }


        /// <summary>
        /// advoid user from setting the scrollbars manually
        /// </summary>
        [Browsable(false)]
        public new ScrollBars ScrollBars
        {
            get { return base.ScrollBars; }
            set { base.ScrollBars = value; }
        }


        #endregion

        #region Declare variables

        public event EventHandler CreateSummary;
        public HScrollBar hScrollBar;
        private SummaryControlContainer summaryControl;
        private Panel panel, spacePanel;
        private TextBox refBox;
        public CheckBox headerCheckBox;
        private Label lbscrollx;
        List<int> listColumnsWidth;
        int headerVisibleX;
        bool firstBind;
        private int sortOrder = 0;
        bool sourceChanged;
        public Dictionary<string, string> summaryValue;
        #endregion

        #region Constructor

        public DataGridViewSummary()
        {
            InitializeComponent();
            firstBind = true;
             listColumnsWidth = new List<int>();
            refBox = new TextBox();
            lbscrollx = new Label();
            headerCheckBox = new CheckBox();
            headerCheckBox.Size = new Size(15,15);
          headerCheckBox.BackColor =Color.Transparent;
           // headerCheckBox.BackColor = Color.Red;
            headerCheckBox.Text = "";
            panel = new Panel();
            spacePanel = new Panel();
            hScrollBar = new HScrollBar();
            //  refBox.Font = new Font(FontFamily.GenericMonospace, 20);
            summaryControl = new SummaryControlContainer(this);
            summaryControl.VisibilityChanged += new EventHandler(summaryControl_VisibilityChanged);

          
            DataSourceChanged += DataGridViewSummary_DataSourceChanged;
            // CellPainting += DataGridViewSummary_CellPainting;
            Resize += new EventHandler(DataGridControlSum_Resize);
            RowHeadersWidthChanged += new EventHandler(DataGridControlSum_Resize);
            //   ColumnAdded += new DataGridViewColumnEventHandler(DataGridControlSum_ColumnAdded);
            //  ColumnRemoved += new DataGridViewColumnEventHandler(DataGridControlSum_ColumnRemoved);
           
          
            hScrollBar.Scroll += new ScrollEventHandler(hScrollBar_Scroll);
            hScrollBar.VisibleChanged += new EventHandler(hScrollBar_VisibleChanged);

            headerCheckBox.Click += HeaderCheckBox_Click;
          
            AllowUserToAddRows = false;
            BackgroundColor = Color.WhiteSmoke;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            summaryValue = new Dictionary<string, string>();
        }

        

        private void DataGridViewSummary_DataSourceChanged(object sender, EventArgs e)
        {
            
            SuspendLayout();
            if (firstBind)
            {
                firstBind = false;


              //  AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;//自动调整单元格大小适应内容
                SelectionMode = DataGridViewSelectionMode.FullRowSelect;//单击选择整行
                MultiSelect = false;//不允许多选单元格

               // ColumnHeadersHeight = cellFontSize+15;
              
                ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;//列标题居中显示
                ColumnHeadersDefaultCellStyle.BackColor = Color.Silver;
               
                RowsDefaultCellStyle.BackColor = Color.White;
                AlternatingRowsDefaultCellStyle.BackColor = Color.LightBlue;
                ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("宋体", headerFontSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                RowsDefaultCellStyle.Font = new Font("宋体", cellFontSize);
                AllowUserToResizeRows = false;

                if (hasCheckBoxColumn)
                {
                    Columns[0].MinimumWidth = 50;
                    Columns[0].HeaderText = "";
                    Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

                    int rowHeaderWidth = RowHeadersVisible ? RowHeadersWidth - 1 : 0;
                    int x = Columns[0].Width / 2 - headerCheckBox.Width / 2 + rowHeaderWidth;
                    int y = ColumnHeadersHeight / 2 - headerCheckBox.Height / 2;
                    headerCheckBox.Location = new Point(x, y);
                    headerVisibleX = Columns[0].Width / 2;
                    headerCheckBox.Visible = true;


                }



            }

            Graphics gh = this.CreateGraphics();
            SizeF sf = gh.MeasureString(RowCount.ToString(), RowsDefaultCellStyle.Font);

            RowHeadersWidth = (int)sf.Width + 20;

            if (summaryRowVisible && summaryColumns != null)
            {
                foreach (DataGridViewColumn dgvColumn in Columns)
                {
                    if (summaryColumns.Contains(dgvColumn.Name))
                    {
                        ReadOnlyTextBox sumBox = (ReadOnlyTextBox)summaryControl.sumBoxHash[dgvColumn];

                        sf = gh.MeasureString(sumBox.Text, RowsDefaultCellStyle.Font);
                        dgvColumn.MinimumWidth = (int)sf.Width + 10;
                    }
                }
            }
            AutoResizeColumns();
            calculateColumnsWidth();
            ResumeLayout(false);
        }
 

        private void HeaderCheckBox_Click(object sender, EventArgs e)
        {
            this.CurrentCell = null;
            foreach (DataGridViewRow dr in this.Rows)
            {
                dr.Cells[0].Value = headerCheckBox.Checked;
            }
        }

 

        private void AjustScroll()
        {
            

            if (columnsWidth > 0&&Width>0)
            {
                int vscrollbarWidth = 0;
                if (VerticalScrollBar.Visible)
                    vscrollbarWidth = VerticalScrollBar.Width;
                int rowHeaderWith = RowHeadersVisible ? RowHeadersWidth : 0;

                hScrollBar.Minimum = 0;
                hScrollBar.Maximum = columnsWidth - 1;

                int scrollBarWidth =  Width - RowHeadersWidth - vscrollbarWidth- columnsWidth;

                if (scrollBarWidth >= 0  )
                {
                    if (hScrollBar.Visible)
                    {
                        hScrollBar.Visible = false;
                    }
                }
                else  
                {
                    if (!hScrollBar.Visible)
                    {
                        hScrollBar.Visible = true;
                    }
                   
                }
                hScrollBar.SmallChange = 0;
                int result = 0;
                if (RowHeadersVisible)
                {
                    result = Width - RowHeadersWidth - 2;
                }
                else
                {
                    result = Width - 3;
                }
                if (result > 0)
                    hScrollBar.LargeChange = result;

            }


        }

        #endregion

        #region public functions

        /// <summary>
        /// Refresh the summary
        /// </summary>
        public void RefreshSummary()
        {
            if (this.summaryControl != null)
                this.summaryControl.RefreshSummary();
        }

        #endregion

        #region Calculate Columns and Scrollbars width
        private void DataGridControlSum_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            calculateColumnsWidth();
            summaryControl.Width = columnsWidth;
            hScrollBar.Maximum = Convert.ToInt32(columnsWidth);
            resizeHScrollBar();
        }
        private void DataGridControlSum_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            calculateColumnsWidth();
            summaryControl.Width = columnsWidth;
            hScrollBar.Maximum = Convert.ToInt32(columnsWidth);
            resizeHScrollBar(); 
        }
        private void  ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            calculateColumnsWidth();
            summaryControl.Width = columnsWidth;
            hScrollBar.Maximum = Convert.ToInt32(columnsWidth);
            resizeHScrollBar(); 
        }

        int columnsWidth = 0;
        /// <summary>
        /// Calculate the width of all visible columns
        /// </summary>
        private void calculateColumnsWidth()
        {
            columnsWidth = 0;
            //listColumnsWidth.Clear();
           // listColumnsWidth.Add(columnsWidth);
            for (int iCnt = 0; iCnt < ColumnCount; iCnt++)
            {
                if (Columns[iCnt].Visible)
                {
                    if (Columns[iCnt].AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                    {
                        columnsWidth += Columns[iCnt].MinimumWidth;
                    }
                    else
                    {
                        columnsWidth += Columns[iCnt].Width;
                    }
                  //  listColumnsWidth.Add(columnsWidth);
                }
            }
            AjustScroll();


        }

        #endregion

        #region Other Events and delegates

        /// <summary>
        /// Moves viewable area of DataGridView according to the position of the scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            //int position = Convert.ToInt32((Convert.ToDouble(e.NewValue) / (Convert.ToDouble(hScrollBar.Maximum) / Convert.ToDouble(Columns.Count))));
            //if (position < Columns.Count)
            //    FirstDisplayedScrollingColumnIndex = position;
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                 
                if (e.NewValue > headerVisibleX)
                {
                    if (headerCheckBox.Visible)
                        headerCheckBox.Visible = false;
                }
                else
                {
                    if (!headerCheckBox.Visible)
                        headerCheckBox.Visible = true;
                }
                HorizontalScrollingOffset = e.NewValue;
            }
        }

        
        public void CreateSummaryRow()
        {
            OnCreateSummary(this, EventArgs.Empty);
        }

        /// <summary>
        /// Calls the CreateSummary event
        /// </summary>
        private void OnCreateSummary(object sender, EventArgs e)
        {
            if (CreateSummary != null)
                CreateSummary(sender, e);
        }

        #endregion

        #region Adjust summaryControl, scrollbar

        /// <summary>
        /// Position the summaryControl under the
        /// DataGridView
        /// </summary>
        private void adjustSumControlToGrid()
        {
            if (summaryControl == null || Parent == null)
                return;
            summaryControl.Top = Height + summaryRowSpace;
            summaryControl.Left = Left;
            summaryControl.Width = Width;
        }

        /// <summary>
        /// Position the hScrollbar under the summaryControl
        /// </summary>
        private void adjustScrollbarToSummaryControl()
        {
            if (Parent != null)
            {
                hScrollBar.Top = refBox.Height + 2;
                hScrollBar.Width = Width;
                hScrollBar.Left = Left;

               
            }
        }

        /// <summary>
        /// Resizes the horizontal scrollbar acording
        /// to the with of the client size and maximum size of the scrollbar
        /// </summary>
        private void resizeHScrollBar()
        {
            //Is used to calculate the LageChange of the scrollbar
            int vscrollbarWidth = 0;
            if (VerticalScrollBar.Visible)
                vscrollbarWidth = VerticalScrollBar.Width;

            int rowHeaderWith = RowHeadersVisible ? RowHeadersWidth : 0;

            if (columnsWidth > 0)
            {
                //This is neccessary if AutoGenerateColumns = true because DataGridControlSum_ColumnAdded won't be fired
                if (hScrollBar.Maximum == 0)
                    hScrollBar.Maximum = columnsWidth;

                //Calculate how much of the columns are visible in %
                int scrollBarWidth = Convert.ToInt32(Convert.ToDouble(ClientSize.Width - RowHeadersWidth - vscrollbarWidth) / (Convert.ToDouble(hScrollBar.Maximum) / 100F));

                if (scrollBarWidth > 100 || columnsWidth + rowHeaderWith < ClientSize.Width)
                {
                    if (hScrollBar.Visible)
                    {
                        hScrollBar.Visible = false;
                    }
                }
                else if (scrollBarWidth > 0)
                {
                    if (!hScrollBar.Visible)
                    {
                        hScrollBar.Visible = true;
                    }
                    hScrollBar.LargeChange = hScrollBar.Maximum / 100 * scrollBarWidth;
                    hScrollBar.SmallChange = hScrollBar.LargeChange / 5;
                }
            }
        }

        private void DataGridControlSum_Resize(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                calculateColumnsWidth();
                resizeHScrollBar();
                AjustScroll();
                adjustSumControlToGrid();
                adjustScrollbarToSummaryControl();
                 
            }
        }

        /// <summary>
        /// Recalculate the width of the summary control according to 
        /// the state of the scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hScrollBar_VisibleChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                //only perform operation if parent is visible
                if (Parent.Visible)
                {
                    int height;
                    if (hScrollBar.Visible)
                        height = summaryControl.InitialHeight + hScrollBar.Height;
                    else
                        height = summaryControl.InitialHeight+4;

                    if (summaryControl.Height != height && summaryControl.Visible)
                    {
                        summaryControl.Height = height;
                        this.Height = panel.Height - summaryControl.Height - summaryRowSpace;
                    }
                }
            }
        }

        /// <summary>
        /// Recalculate the height of the DataGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void summaryControl_VisibilityChanged(object sender, EventArgs e)
        {
            if (!summaryControl.Visible)
            {
                ScrollBars = ScrollBars.Both;
                this.Height = panel.Height;
            }
            else
            {
                this.Height = panel.Height - summaryControl.Height - summaryRowSpace;
                ScrollBars = ScrollBars.Vertical;
            }
        }

        /// <summary>
        /// When the DataGridView is visible for the first time a panel is created.
        /// The DataGridView is then removed from the parent control and added as 
        /// child to the newly created panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeParent()
        {
            if (!DesignMode && Parent != null)
            { 

                summaryControl.InitialHeight = this.refBox.Height;
                summaryControl.Height = summaryControl.InitialHeight;
                summaryControl.BackColor = this.RowHeadersDefaultCellStyle.BackColor;
                summaryControl.ForeColor = Color.Transparent;
                summaryControl.RightToLeft = this.RightToLeft;
                panel.Bounds = this.Bounds;
                panel.BackColor = this.BackgroundColor;

                panel.Dock = this.Dock;
                panel.Anchor = this.Anchor;
                panel.Padding = this.Padding;
                panel.Margin = this.Margin;
                panel.Top = this.Top;
                panel.Left = this.Left;
                panel.BorderStyle = this.BorderStyle;
                //panel.BorderStyle = BorderStyle.FixedSingle;
                //summaryControl.BackColor = Color.Red;
                Margin = new Padding(0);
                Padding = new Padding(0);
                Top = 0;
                Left = 0;

                summaryControl.Dock = DockStyle.Bottom;
                this.Dock = DockStyle.Fill;

                if (this.Parent is TableLayoutPanel)
                {
                    int rowSpan, colSpan;

                    TableLayoutPanel tlp = this.Parent as TableLayoutPanel;
                    TableLayoutPanelCellPosition cellPos = tlp.GetCellPosition(this);

                    rowSpan = tlp.GetRowSpan(this);
                    colSpan = tlp.GetColumnSpan(this);

                    tlp.Controls.Remove(this);
                    tlp.Controls.Add(panel, cellPos.Column, cellPos.Row);
                    tlp.SetRowSpan(panel, rowSpan);
                    tlp.SetColumnSpan(panel, colSpan);
                }
                else
                {
                    Control parent = this.Parent;
                    //remove DataGridView from ParentControls
                    parent.Controls.Remove(this);
                    parent.Controls.Add(panel);
                }

                this.BorderStyle = BorderStyle.None;

                panel.BringToFront();


                hScrollBar.Top = refBox.Height + 2;
                hScrollBar.Width = this.Width;
                hScrollBar.Left = this.Left;

                summaryControl.Controls.Add(hScrollBar);
                hScrollBar.BringToFront();
                panel.Controls.Add(this);

                spacePanel = new Panel();
                spacePanel.BackColor = panel.BackColor;

                spacePanel.Height = summaryRowSpace;
                spacePanel.Dock = DockStyle.Bottom;

                panel.Controls.Add(spacePanel);
                panel.Controls.Add(summaryControl);
 
                if(hasCheckBoxColumn)
                {

                    this.Controls.Add(headerCheckBox);
                    headerCheckBox.Visible = false;
                }
                if (RowHeadersVisible)
                {
                    //RowStateChanged += DataGridViewSummary_RowStateChanged;
                    RowPostPaint += DataGridViewSummary_RowPostPaint;
                }

                RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
              
                adjustSumControlToGrid();
                adjustScrollbarToSummaryControl();
                
                
            }
        }

        private void DataGridViewSummary_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
          //  e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
            using (SolidBrush b = new SolidBrush( RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex+1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + e.RowBounds.Height/2 - e.InheritedRowStyle.Font.Height / 2+1);
            }
        }

        #endregion

        #region ISupportInitialzie

        public void BeginInit()
        { }

        public void EndInit()
        {
            changeParent();
        }

        #endregion
    }
}
