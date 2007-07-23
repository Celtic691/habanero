using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Habanero.UI.Grid
{
    /// <summary>
    /// A control for editing date and time values
    /// </summary>
    class CalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
    {
        DataGridView _dataGridView;
        private bool _valueChanged = false;
        int _rowIndex;

        /// <summary>
        /// Constructor to initialise a new editing control with the short
        /// date format
        /// </summary>
        public CalendarEditingControl()
        {
            this.Format = DateTimePickerFormat.Short;
        }

        /// <summary>
        /// Gets and sets the value being held in the control
        /// </summary>
        public object EditingControlFormattedValue
        {
            get
            {
                return this.Value.ToShortDateString();
            }
            set
            {
                if (value is String)
                {
                    this.Value = DateTime.Parse((String)value);
                }
            }
        }

        /// <summary>
        /// Returns the value being held in the control
        /// </summary>
        /// <returns>Returns the value being held</returns>
        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        /// <summary>
        /// Copy the styles from the object provided across to this editing
        /// control
        /// </summary>
        /// <param name="dataGridViewCellStyle">The source to copy from</param>
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        /// <summary>
        /// Gets and sets the row index number
        /// </summary>
        public int EditingControlRowIndex
        {
            get
            {
                return _rowIndex;
            }
            set
            {
                _rowIndex = value;
            }
        }

        /// <summary>
        /// Indicates if the editing control wants the input key specified
        /// </summary>
        /// <param name="key">The key in question</param>
        /// <param name="dataGridViewWantsInputKey">Whether the DataGridView
        /// wants the input key</param>
        /// <returns>Returns true if so, false if not</returns>
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the DateTimePicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Prepares the editing control for editing
        /// </summary>
        /// <param name="selectAll">Whether to select all the content first,
        /// which can make it easier to replace as you type</param>
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        /// <summary>
        /// Indicates whether the control should be repositioned when there
        /// is a value change
        /// </summary>
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets and sets the DataGridView object referenced in this
        /// control
        /// </summary>
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return _dataGridView;
            }
            set
            {
                _dataGridView = value;
            }
        }

        /// <summary>
        /// Gets and sets the boolean which indicates whether the value
        /// held in the control has changed
        /// </summary>
        public bool EditingControlValueChanged
        {
            get
            {
                return _valueChanged;
            }
            set
            {
                _valueChanged = value;
            }
        }

        /// <summary>
        /// Returns the Cursor object from the editing panel
        /// </summary>
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        /// <summary>
        /// A handler to carry out repercussions of a changed value
        /// </summary>
        /// <param name="eventargs">Arguments relating to the event</param>
        protected override void OnValueChanged(EventArgs eventargs)
        {
            // Notify the DataGridView that the contents of the cell
            // have changed.
            _valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }
}