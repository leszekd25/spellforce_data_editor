using SFEngine.SFCFF;
using System;
using System.Collections.Generic;


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
            {
                SubOperators[i].OnApplyUpdateUI();
            }
        }

        public void OnRevertUpdateUI()
        {
            for (int i = SubOperators.Count - 1; i >= 0; i--)
            {
                SubOperators[i].OnRevertUpdateUI();
            }
        }

        public void Apply()
        {
            for (int i = 0; i < SubOperators.Count; i++)
            {
                SubOperators[i].Apply();
            }
        }

        public void Revert()
        {
            for (int i = SubOperators.Count - 1; i >= 0; i--)
            {
                SubOperators[i].Revert();
            }
        }
        public override string ToString()
        {
            return "cluster (" + SubOperators.Count + " sub-operators)";
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
            {
                MainForm.data.OperatorCategoryRemoved(CategoryID, CategoryType);
            }
            else
            {
                MainForm.data.OperatorCategoryAdded(CategoryID, CategoryType);
            }
        }

        public void OnRevertUpdateUI()
        {
            if (IsRemoving)
            {
                MainForm.data.OperatorCategoryAdded(CategoryID, CategoryType);
            }
            else
            {
                MainForm.data.OperatorCategoryRemoved(CategoryID, CategoryType);
            }
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
            {
                SFCategoryManager.gamedata.categories.Add(Category.category_id, Category);
            }
            else
            {
                SFCategoryManager.gamedata.categories.Remove(CategoryID);
            }
        }


        public override string ToString()
        {
            if (IsRemoving)
            {
                return string.Format("remove category ({0}, {1})", CategoryID, CategoryType);
            }
            else
            {
                return string.Format("add category ({0}, {1})", CategoryID, CategoryType);
            }
        }
    }

    public class CFFOperatorAddRemoveCategoryElement : ICFFOperator
    {
        public SFCategoryElement Element;
        public SFCategoryElementList ElementList;        // only if IsElementList
        public SFCategoryElementStatus ElementStatus;    // read only
        public SFCategoryElementStatus SubElementStatus; // read only
        public int CategoryIndex;
        public int ElementIndex;
        public int SubElementIndex;
        public bool IsElementList = false;
        public bool IsSubElement = false;
        public bool IsRemoving = false;

        public void OnApplyUpdateUI()
        {
            if (IsRemoving)
            {
                MainForm.data.OperatorElementRemoved((ushort)CategoryIndex, ElementIndex, (IsSubElement ? SubElementIndex : -1));
            }
            else
            {
                MainForm.data.OperatorElementAdded((ushort)CategoryIndex, ElementIndex, (IsSubElement ? SubElementIndex : -1));
            }
        }

        public void OnRevertUpdateUI()
        {
            if (IsRemoving)
            {
                MainForm.data.OperatorElementAdded((ushort)CategoryIndex, ElementIndex, (IsSubElement ? SubElementIndex : -1));
            }
            else
            {
                MainForm.data.OperatorElementRemoved((ushort)CategoryIndex, ElementIndex, (IsSubElement ? SubElementIndex : -1));
            }
        }

        public void Apply()
        {
            if (IsRemoving)
            {
                if (IsSubElement)
                {
                    Element = SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex];
                    ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                    SubElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex];
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements.RemoveAt(SubElementIndex);
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus.RemoveAt(SubElementIndex);
                    if (ElementStatus != SFCategoryElementStatus.ADDED)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                    }
                }
                else
                {
                    if (IsElementList)
                    {
                        ElementList = SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex];
                        SFCategoryManager.gamedata[CategoryIndex].element_lists.RemoveAt(ElementIndex);
                    }
                    else
                    {
                        Element = SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex];
                        SFCategoryManager.gamedata[CategoryIndex].elements.RemoveAt(ElementIndex);
                    }

                    ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                    SFCategoryManager.gamedata[CategoryIndex].element_status.RemoveAt(ElementIndex);
                }
            }
            else
            {
                if (IsSubElement)
                {
                    ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements.Insert(SubElementIndex, Element);
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus.Insert(SubElementIndex, SFCategoryElementStatus.ADDED);
                    // if element was added, it stays added
                    if (ElementStatus != SFCategoryElementStatus.ADDED)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                    }
                }
                else
                {
                    if (IsElementList)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_lists.Insert(ElementIndex, ElementList);
                    }
                    else
                    {
                        SFCategoryManager.gamedata[CategoryIndex].elements.Insert(ElementIndex, Element);
                    }

                    SFCategoryManager.gamedata[CategoryIndex].element_status.Insert(ElementIndex, SFCategoryElementStatus.ADDED);
                }
            }
        }

        public void Revert()
        {
            if (IsRemoving)
            {
                if (IsSubElement)
                {
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements.Insert(SubElementIndex, Element);
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus.Insert(SubElementIndex, SubElementStatus);
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
                }
                else
                {
                    if (IsElementList)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_lists.Insert(ElementIndex, ElementList);
                    }
                    else
                    {
                        SFCategoryManager.gamedata[CategoryIndex].elements.Insert(ElementIndex, Element);
                    }

                    SFCategoryManager.gamedata[CategoryIndex].element_status.Insert(ElementIndex, ElementStatus);
                }
            }
            else
            {
                if (IsSubElement)
                {
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements.RemoveAt(SubElementIndex);
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus.RemoveAt(SubElementIndex);
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
                }
                else
                {
                    if (IsElementList)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_lists.RemoveAt(ElementIndex);
                    }
                    else
                    {
                        SFCategoryManager.gamedata[CategoryIndex].elements.RemoveAt(ElementIndex);
                    }

                    SFCategoryManager.gamedata[CategoryIndex].element_status.RemoveAt(ElementIndex);
                }

            }
        }

        public override string ToString()
        {
            if (IsRemoving)
            {
                if (IsSubElement)
                {
                    return string.Format("remove element at [{0}, {1}] in category {2}", ElementIndex, SubElementIndex, CategoryIndex);
                }
                else if (IsElementList)
                {
                    return string.Format("remove element list at {0} in category {1}", ElementIndex, CategoryIndex);
                }
                else
                {
                    return string.Format("remove element at {0} in category {1}", ElementIndex, CategoryIndex);
                }
            }
            else
            {
                if (IsSubElement)
                {
                    return string.Format("add element at [{0}, {1}] in category {2}", ElementIndex, SubElementIndex, CategoryIndex);
                }
                else if (IsElementList)
                {
                    return string.Format("add element list at {0} in category {1}", ElementIndex, CategoryIndex);
                }
                else
                {
                    return string.Format("add element at {0} in category {1}", ElementIndex, CategoryIndex);
                }
            }
        }
    }

    // todo: history
    public class CFFOperatorModifyCategoryElement : ICFFOperator
    {
        public int CategoryIndex;
        public int ElementIndex;
        public int SubElementIndex;
        public int VariantIndex;
        public object OldVariant;
        public object NewVariant;
        public SFCategoryElementStatus ElementStatus;
        public SFCategoryElementStatus SubElementStatus;
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
                ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                SubElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex];
                SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex] = NewVariant;
                if (ElementStatus != SFCategoryElementStatus.ADDED)
                {
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                }

                if (SubElementStatus != SFCategoryElementStatus.ADDED)
                {
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex] = SFCategoryElementStatus.MODIFIED;
                }
            }
            else
            {
                OldVariant = SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex];
                ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex] = NewVariant;
                if (ElementStatus != SFCategoryElementStatus.ADDED)
                {
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                }
            }
        }

        public void Revert()
        {
            if (IsSubElement)
            {
                SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex] = OldVariant;
                SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex] = SubElementStatus;
                SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
            }
            else
            {
                SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex] = OldVariant;
                SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
            }
        }

        public override string ToString()
        {
            if (IsSubElement)
            {
                return string.Format("modify element at [{0}, {1}] in category {2}", ElementIndex, SubElementIndex, CategoryIndex);
            }
            else
            {
                return string.Format("modify element at {0} in category {1}", ElementIndex, CategoryIndex);
            }
        }
    }

    // todo: history
    public class CFFOperatorAddRemoveCategoryElementOutlineData : ICFFOperator
    {
        public int CategoryIndex;
        public int ElementIndex;
        public int SubElementIndex;
        public int VariantIndex;
        public int VertexIndex;
        public short X, Y;
        public SFCategoryElementStatus ElementStatus;
        public SFCategoryElementStatus SubElementStatus;
        public bool IsSubElement = false;
        public bool IsRemoving = false;

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
            if (IsRemoving)
            {
                if (IsSubElement)
                {
                    X = ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 0];
                    Y = ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 1];
                    ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                    SubElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex];
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data.RemoveAt(VertexIndex * 2 + 0);
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data.RemoveAt(VertexIndex * 2 + 0);
                    if (ElementStatus != SFCategoryElementStatus.ADDED)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                    }

                    if (SubElementStatus != SFCategoryElementStatus.ADDED)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex] = SFCategoryElementStatus.MODIFIED;
                    }
                }
                else
                {
                    X = ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 0];
                    Y = ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 1];
                    ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data.RemoveAt(VertexIndex * 2 + 0);
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data.RemoveAt(VertexIndex * 2 + 0);
                    if (ElementStatus != SFCategoryElementStatus.ADDED)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                    }
                }
            }
            else
            {
                if (IsSubElement)
                {
                    ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                    SubElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex];
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data.Insert(VertexIndex * 2 + 0, Y);
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data.Insert(VertexIndex * 2 + 0, X);
                    if (ElementStatus != SFCategoryElementStatus.ADDED)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                    }

                    if (SubElementStatus != SFCategoryElementStatus.ADDED)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex] = SFCategoryElementStatus.MODIFIED;
                    }
                }
                else
                {
                    ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data.Insert(VertexIndex * 2 + 0, Y);
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data.Insert(VertexIndex * 2 + 0, X);
                    if (ElementStatus != SFCategoryElementStatus.ADDED)
                    {
                        SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                    }
                }
            }
        }

        public void Revert()
        {
            if (IsRemoving)
            {
                if (IsSubElement)
                {
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data.Insert(VertexIndex * 2 + 0, Y);
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data.Insert(VertexIndex * 2 + 0, X);
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex] = SubElementStatus;
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
                }
                else
                {
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data.Insert(VertexIndex * 2 + 0, Y);
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data.Insert(VertexIndex * 2 + 0, X);
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
                }
            }
            else
            {
                if (IsSubElement)
                {
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data.RemoveAt(VertexIndex * 2 + 0);
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data.RemoveAt(VertexIndex * 2 + 0);
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex] = SubElementStatus;
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
                }
                else
                {
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data.RemoveAt(VertexIndex * 2 + 0);
                    ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data.RemoveAt(VertexIndex * 2 + 0);
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
                }
            }
        }

        public override string ToString()
        {
            if (IsRemoving)
            {
                if (IsSubElement)
                {
                    return string.Format("remove vertex {0} at element [{1}, {2}] in category {3}", VertexIndex, ElementIndex, SubElementIndex, CategoryIndex);
                }
                else
                {
                    return string.Format("remove vertex {0} at element {1} in category {2}", VertexIndex, ElementIndex, CategoryIndex);
                }
            }
            else
            {
                if (IsSubElement)
                {
                    return string.Format("add vertex {0} at element [{1}, {2}] in category {3}", VertexIndex, ElementIndex, SubElementIndex, CategoryIndex);
                }
                else
                {
                    return string.Format("add vertex {0} at element {1} in category {2}", VertexIndex, ElementIndex, CategoryIndex);
                }
            }
        }
    }

    // todo: history
    public class CFFOperatorModifyCategoryElementOutlineData : ICFFOperator
    {
        public int CategoryIndex;
        public int ElementIndex;
        public int SubElementIndex;
        public int VariantIndex;
        public int VertexIndex;
        public short OldX, OldY;
        public short NewX, NewY;
        public SFCategoryElementStatus ElementStatus;
        public SFCategoryElementStatus SubElementStatus;
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
                OldX = ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 0];
                OldY = ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 1];
                ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                SubElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex];
                ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 0] = NewX;
                ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 1] = NewY;
                if (ElementStatus != SFCategoryElementStatus.ADDED)
                {
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                }

                if (SubElementStatus != SFCategoryElementStatus.ADDED)
                {
                    SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex] = SFCategoryElementStatus.MODIFIED;
                }
            }
            else
            {
                OldX = ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 0];
                OldY = ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 1];
                ElementStatus = SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex];
                ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 0] = NewX;
                ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 1] = NewY;
                if (ElementStatus != SFCategoryElementStatus.ADDED)
                {
                    SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = SFCategoryElementStatus.MODIFIED;
                }
            }
        }

        public void Revert()
        {
            if (IsSubElement)
            {
                ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 0] = OldX;
                ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].Elements[SubElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 1] = OldY;
                SFCategoryManager.gamedata[CategoryIndex].element_lists[ElementIndex].ElementStatus[SubElementIndex] = SubElementStatus;
                SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
            }
            else
            {
                ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 0] = OldX;
                ((SFOutlineData)(SFCategoryManager.gamedata[CategoryIndex].elements[ElementIndex].variants[VariantIndex])).Data[VertexIndex * 2 + 1] = OldY;
                SFCategoryManager.gamedata[CategoryIndex].element_status[ElementIndex] = ElementStatus;
            }
        }
    }
}
