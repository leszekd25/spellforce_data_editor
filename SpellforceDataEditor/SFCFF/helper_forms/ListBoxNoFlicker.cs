using System.Drawing;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.helper_forms
{
    // http://yacsharpblog.blogspot.com/2008/07/listbox-flicker.html
    internal class ListBoxNoFlicker : System.Windows.Forms.ListBox
    {
        public ListBoxNoFlicker()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            DrawMode = DrawMode.OwnerDrawFixed;
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if ((Items.Count > 0) && (e.Index != -1))
            {
                e.DrawBackground();
                e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));
            }
            base.OnDrawItem(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(BackColor), iRegion);
            if (Items.Count > 0)
            {
                System.Drawing.Rectangle rct = GetItemRectangle(0);
                int first_pos = -(rct.Y / rct.Height);
                int item_count = Height / rct.Height;
                for (int i = first_pos; i < first_pos + item_count; ++i)
                {
                    if (i >= Items.Count)
                    {
                        break;
                    }

                    System.Drawing.Rectangle irect = GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        if ((SelectionMode == SelectionMode.One && SelectedIndex == i)
                        || (SelectionMode == SelectionMode.MultiSimple && SelectedIndices.Contains(i))
                        || (SelectionMode == SelectionMode.MultiExtended && SelectedIndices.Contains(i)))
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font,
                                irect, i,
                                DrawItemState.Selected, ForeColor,
                                BackColor));
                        }
                        else
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font,
                                irect, i,
                                DrawItemState.Default, ForeColor,
                                BackColor));
                        }
                        iRegion.Complement(irect);
                    }
                }
            }
            base.OnPaint(e);
        }
    }
}
