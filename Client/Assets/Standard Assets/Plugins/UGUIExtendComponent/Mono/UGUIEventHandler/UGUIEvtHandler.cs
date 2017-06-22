using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace UnityEngine.EventSystems
{
    public enum UGUIEvtType
    {
        BEGIN_DRAG,
        CANCEL,
        DESELECT,
        DRAG,
        DROP,
        END_DRAG,
        INIT_POTENTIAL_DRAG,
        MOVE,
        POINTER_CLICK,
        POINTER_DOWN,
        POINTER_UP,
        POINTER_ENTER,
        POINTER_EXIT,
        SCROLL,
        SELECT,
        SUBMIT,
        UPDATE_SELECTED
    }

    public class UGUIEvtHandler :
        MonoBehaviour,
        IBeginDragHandler,
        ICancelHandler,
        IDeselectHandler,
        IDragHandler,
        IDropHandler,
        IEndDragHandler,
        IMoveHandler,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IEventSystemHandler,
        IInitializePotentialDragHandler,
        IScrollHandler,
        ISelectHandler,
        ISubmitHandler,
        IUpdateSelectedHandler
    {
        public delegate void EVENT_CALLBACK<T>(T eventData);

        public EVENT_CALLBACK<PointerEventData> FuncOnBeginDrag = null;
        public EVENT_CALLBACK<BaseEventData> FuncOnCancel = null;
        public EVENT_CALLBACK<BaseEventData> FuncOnDeselect = null;
        public EVENT_CALLBACK<PointerEventData> FuncOnDrag = null;
        public EVENT_CALLBACK<PointerEventData> FuncOnDrop = null;
        public EVENT_CALLBACK<PointerEventData> FuncOnEndDrag = null;
        public EVENT_CALLBACK<AxisEventData> FuncOnMove = null;

        public EVENT_CALLBACK<PointerEventData> FuncOnPointerClick = null;
        public EVENT_CALLBACK<PointerEventData> FuncOnPointerDown = null;
        public EVENT_CALLBACK<PointerEventData> FuncOnPointerUp = null;
        public EVENT_CALLBACK<PointerEventData> FuncOnPointerEnter = null;
        public EVENT_CALLBACK<PointerEventData> FuncOnPointerExit = null;

        public EVENT_CALLBACK<PointerEventData> FuncOnInitPotentialDrag = null;
        public EVENT_CALLBACK<PointerEventData> FuncOnScroll = null;
        public EVENT_CALLBACK<BaseEventData> FuncOnSelect = null;
        public EVENT_CALLBACK<BaseEventData> FuncOnSubmit = null;
        public EVENT_CALLBACK<BaseEventData> FuncOnUpdateSelected = null;




        static public void AddListener(GameObject obj, UGUIEvtType evtType, EVENT_CALLBACK<BaseEventData> func)
        {
            AddListenerAsDelegate(obj, evtType, func);
        }

        static public void AddListener(GameObject obj, UGUIEvtType evtType, EVENT_CALLBACK<PointerEventData> func)
        {
            AddListenerAsDelegate(obj, evtType, func);
        }

        static public void AddListener(GameObject obj, UGUIEvtType evtType, EVENT_CALLBACK<AxisEventData> func)
        {
            AddListenerAsDelegate(obj, evtType, func);
        }

        static public void AddListenerAsDelegate(GameObject obj, UGUIEvtType evtType, Delegate func)
        {
            UGUIEvtHandler handler = obj.GetComponent<UGUIEvtHandler>();
            if (handler == null)
            {
                handler = obj.AddComponent<UGUIEvtHandler>();
            }
            EVENT_CALLBACK<BaseEventData> bHandleFunc;
            EVENT_CALLBACK<PointerEventData> pHandleFunc;
            EVENT_CALLBACK<AxisEventData> aHandleFunc;
            switch (evtType)
            {
                case UGUIEvtType.BEGIN_DRAG:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event BEGIN_DRAG Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnBeginDrag += pHandleFunc;
                    break;
                case UGUIEvtType.CANCEL:
                    bHandleFunc = func as EVENT_CALLBACK<BaseEventData>;
                    if (bHandleFunc == null)
                    {
                        throw new Exception("Event CANCEL Need a EVENT_CALLBACK<BaseEventData> instance as callback function");
                    }
                    handler.FuncOnCancel += bHandleFunc;
                    break;
                case UGUIEvtType.DESELECT:
                    bHandleFunc = func as EVENT_CALLBACK<BaseEventData>;
                    if (bHandleFunc == null)
                    {
                        throw new Exception("Event DESELECT Need a EVENT_CALLBACK<BaseEventData> instance as callback function");
                    }
                    handler.FuncOnDeselect += bHandleFunc;
                    break;
                case UGUIEvtType.DRAG:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event DRAG Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnDrag += pHandleFunc;
                    break;
                case UGUIEvtType.DROP:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event DROP Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnDrop += pHandleFunc;
                    break;
                case UGUIEvtType.END_DRAG:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event END_DRAG Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnEndDrag += pHandleFunc;
                    break;
                case UGUIEvtType.INIT_POTENTIAL_DRAG:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event INIT_POTENTIAL_DRAG Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnInitPotentialDrag += pHandleFunc;
                    break;
                case UGUIEvtType.MOVE:
                    aHandleFunc = func as EVENT_CALLBACK<AxisEventData>;
                    if (aHandleFunc == null)
                    {
                        throw new Exception("Event MOVE Need a EVENT_CALLBACK<AxisEventData> instance as callback function");
                    }
                    handler.FuncOnMove += aHandleFunc;
                    break;
                case UGUIEvtType.POINTER_CLICK:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event POINTER_CLICK Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnPointerClick += pHandleFunc;
                    break;
                case UGUIEvtType.POINTER_DOWN:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event POINTER_DOWN Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnPointerDown += pHandleFunc;
                    break;
                case UGUIEvtType.POINTER_ENTER:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event POINTER_ENTER Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnPointerEnter += pHandleFunc;
                    break;
                case UGUIEvtType.POINTER_EXIT:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event POINTER_EXIT Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnPointerExit += pHandleFunc;
                    break;
                case UGUIEvtType.POINTER_UP:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event POINTER_UP Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnPointerUp += pHandleFunc;
                    break;
                case UGUIEvtType.SCROLL:
                    pHandleFunc = func as EVENT_CALLBACK<PointerEventData>;
                    if (pHandleFunc == null)
                    {
                        throw new Exception("Event SCROLL Need a EVENT_CALLBACK<PointerEventData> instance as callback function");
                    }
                    handler.FuncOnScroll += pHandleFunc;
                    break;
                case UGUIEvtType.SELECT:
                    bHandleFunc = func as EVENT_CALLBACK<BaseEventData>;
                    if (bHandleFunc == null)
                    {
                        throw new Exception("Event SELECT Need a EVENT_CALLBACK<BaseEventData> instance as callback function");
                    }
                    handler.FuncOnSelect += bHandleFunc;
                    break;
                case UGUIEvtType.SUBMIT:
                    bHandleFunc = func as EVENT_CALLBACK<BaseEventData>;
                    if (bHandleFunc == null)
                    {
                        throw new Exception("Event SUBMIT Need a EVENT_CALLBACK<BaseEventData> instance as callback function");
                    }
                    handler.FuncOnSubmit += bHandleFunc;
                    break;
                case UGUIEvtType.UPDATE_SELECTED:
                    bHandleFunc = func as EVENT_CALLBACK<BaseEventData>;
                    if (bHandleFunc == null)
                    {
                        throw new Exception("Event UPDATE_SELECTED Need a EVENT_CALLBACK<BaseEventData> instance as callback function");
                    }
                    handler.FuncOnUpdateSelected += bHandleFunc;
                    break;
                default:
                    throw new Exception("Wrong Event Type!");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (FuncOnBeginDrag != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnBeginDrag.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (FuncOnCancel != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnCancel.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (FuncOnDeselect != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnDeselect.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (FuncOnDrag != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnDrag.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (FuncOnDrop != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnDrop.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (FuncOnEndDrag != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnEndDrag.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }
        public void OnMove(AxisEventData eventData)
        {
            if (FuncOnMove != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnMove.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (FuncOnPointerClick != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnPointerClick.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (FuncOnPointerDown != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnPointerDown.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (FuncOnPointerUp != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnPointerUp.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (FuncOnPointerEnter != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnPointerEnter.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (FuncOnPointerExit != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnPointerExit.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (FuncOnInitPotentialDrag != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnInitPotentialDrag.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (FuncOnScroll != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnScroll.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (FuncOnSelect != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnSelect.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (FuncOnSubmit != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnSubmit.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            if (FuncOnUpdateSelected != null)
            {
#if !_DEBUG
                try
                {
#endif
                    FuncOnUpdateSelected.Invoke(eventData);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    //ExceptionHandler.LogException(e);
                }
#endif
            }
        }

    }
}