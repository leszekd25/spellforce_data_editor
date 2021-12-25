using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFEngine.SFCFF;


namespace SpellforceDataEditor.SFCFF.operators
{
    public interface ICFFOperator
    {
        // is done after Apply, all UI code should go here
        void OnApplyUpdateUI();
        // is done after Revert, all UI code should go here
        void OnRevertUpdateUI();

        // performs an action given the operator's inputs; gamedata is implicit
        void Apply();
        // performs a reverse of an action given the operator's inputs
        void Revert();
    }

    public class CFFOperatorCluster : ICFFOperator
    {
        public List<ICFFOperator> SubOperators = new List<ICFFOperator>();

        public void OnApplyUpdateUI()
        {
            for (int i = 0; i < SubOperators.Count; i++)
                SubOperators[i].OnApplyUpdateUI();
        }

        public void OnRevertUpdateUI()
        {
            for (int i = SubOperators.Count - 1; i >= 0; i--)
                SubOperators[i].OnRevertUpdateUI();
        }

        public void Apply()
        {
            for (int i = 0; i < SubOperators.Count; i++)
                SubOperators[i].Apply();
        }

        public void Revert()
        {
            for (int i = SubOperators.Count - 1; i >= 0; i--)
                SubOperators[i].Revert();
        }
    }

    public class CFFOperatorAddRemoveCategory : ICFFOperator
    {
        private SFCategory Category;

        public ushort CategoryID;
        public ushort CategoryType;
        public bool IsRemoving = false;

        public void OnApplyUpdateUI()
        {
            if (IsRemoving)
                MainForm.data.OperatorCategoryRemoved(CategoryID);
            else
                MainForm.data.OperatorCategoryAdded(CategoryID);
        }

        public void OnRevertUpdateUI()
        {
            if (IsRemoving)
                MainForm.data.OperatorCategoryAdded(CategoryID);
            else
                MainForm.data.OperatorCategoryRemoved(CategoryID);
        }

        public void Apply()
        {
            if (IsRemoving)
            {
                Category = SFCategoryManager.gamedata.categories[CategoryID];
                SFCategoryManager.gamedata.categories.Remove(CategoryID);
            }
            else
            {
                SFCategory cat = new SFCategory(Tuple.Create<ushort, ushort>(CategoryID, CategoryType));
                SFCategoryManager.gamedata.categories.Add(CategoryID, cat);
            }
        }

        public void Revert()
        {
            if (IsRemoving)
                SFCategoryManager.gamedata.categories.Add(Category.category_id, Category);
            else
                SFCategoryManager.gamedata.categories.Remove(CategoryID);
        }
    }

    public class CFFOperatorAddRemoveCategoryElement : ICFFOperator
    {
        public SFCategoryElement Element;
        public SFCategoryElementStatus ElementStatus;
        public SFCategoryElementStatus SubElementStatus;
        public int CategoryIndex;
        public int ElementIndex;
        public int SubElementIndex;
        public bool IsSubElement = false;
        public bool IsRemoving = false;

        public void OnApplyUpdateUI()
        {
            if (IsRemoving)
                MainForm.data.OperatorElementRemoved((ushort)CategoryIndex, ElementIndex, (IsSubElement ? SubElementIndex : -1));
            else
                MainForm.data.OperatorElementAdded((ushort)CategoryIndex, ElementIndex, (IsSubElement ? SubElementIndex : -1));
        }

        public void OnRevertUpdateUI()
        {
            if (IsRemoving)
                MainForm.data.OperatorElementAdded((ushort)CategoryIndex, ElementIndex, (IsSubElement ? SubElementIndex : -1));
            else
                MainForm.data.OperatorElementRemoved((ushort)CategoryIndex, ElementIndex, (IsSubElement ? SubElementIndex : -1));
        }

        public void Apply()
        {
            if(IsRemoving)
            {
                if (IsSubElement)
                {
                    Element = SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex];
                    ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                    SubElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex];
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements.RemoveAt(SubElementIndex);
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus.RemoveAt(SubElementIndex);
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                }
                else
                {
                    Element = SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex];
                    ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                    SFCategoryManager.gamedata[CategoryIndex].elements.RemoveAt(ElementIndex);
                    SFCategoryManager.gamedata[CategoryIndex].element_status.RemoveAt(ElementIndex);
                }
            }
            else
            {
                if(IsSubElement)
                {
                    ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements.Insert(SubElementIndex, SFCategoryManager.gamedata[CategoryIndex].GetEmptyElement());
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus.Insert(SubElementIndex, SFCategoryElementStatus.ADDED);
                    if(ElementStatus == SFCategoryElementStatus.UNCHANGED)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                    }
                }
                else
                {
                    SFCategoryManager.gamedata[CategoryIndex].elements.Insert(ElementIndex, SFCategoryManager.gamedata[CategoryIndex].GetEmptyElement());
                    SFCategoryManager.gamedata[CategoryIndex].element_status.Insert(ElementIndex, SFCategoryElementStatus.ADDED);
                }
            }
        }

        public void Revert()
        {
            if(IsRemoving)
            {
                if(IsSubElement)
                {
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements.Insert(SubElementIndex, Element);
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus.Insert(SubElementIndex, SubElementStatus);
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
                }
                else
                {
                    SFCategoryManager.gamedata[CategoryIndex].elements.Insert(ElementIndex, Element);
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
                }
            }
            else
            {
                if(IsSubElement)
                {
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements.RemoveAt(SubElementIndex);
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus.RemoveAt(SubElementIndex);
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
                }
                else
                {
                    SFCategoryManager.gamedata[CategoryIndex].elements.RemoveAt(ElementIndex);
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
                }

            }
        }
    }

    public class CFFOperatorModifyCategoryElement : ICFFOperator
    {
        public int CategoryIndex;
        public int ElementIndex;
        public int SubElementIndex;
        public int VariantIndex;
        public object OldVariant;
        public object NewVariant;
        public bool IsSubElement = false;

        public void OnApplyUpdateUI()
        {
            MainForm.data.OperatorElementModified((ushort)CategoryIndex, ElementIndex, (IsSubElement ? SubElementIndex : -1));
        }

        public void OnRevertUpdateUI()
        {
            MainForm.data.OperatorElementModified((ushort)CategoryIndex, ElementIndex, (IsSubElement ? SubElementIndex : -1));
        }

        public void Apply()
        {
            if (IsSubElement)
            {
                OldVariant = SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex];
                SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex] = NewVariant;
            }
            else
            {
                OldVariant = SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex];
                SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex] = NewVariant;
            }
        }

        public void Revert()
        {
            if (IsSubElement)
            {
                SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex] = OldVariant;
            }
            else
            {
                SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex] = OldVariant;
            }
        }
    }
}
