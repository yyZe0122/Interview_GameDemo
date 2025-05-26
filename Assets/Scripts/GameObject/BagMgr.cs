using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BagMgr : BaseManager<BagMgr>
{
    //当前拖动的格子
    private ItemCall nowDragItemCall = new ItemCall();
    //当前鼠标进入的格子
    private ItemCall nowInItemCall = new ItemCall();
    //当前选中装备的图片信息
    private Image nowItemImage;
    //防止异步加载 是否拖动中
    private bool isDrag = false;
    private PlayerData playerData;
    private HeroInfo heroData;

    /// <summary>
    /// 初始化BagMgr
    /// </summary>
    public void Init()
    {
        playerData = GameDataMgr.Instance.playerData;
        heroData = GameDataMgr.Instance.heroData;
        //鼠标进入
        EventCenter.Instance.AddEventListener<ItemCall>(E_EventType.E_Bag_PointerEnter, (data) =>
        {
            Debug.Log("鼠标进入id" + data.NowItemCallID);
            //如果在拖动 那么不需要详情面板
            if (isDrag)
            {
                //记录当前进入的格子
                nowInItemCall = data;
                return;
            }
            //显示tips面板
            //判断是不是装备栏
            if (data.NowItemCallID == -1 || data.NowItemCallID == -2)
            {
                //判断有没有装备
                //如果没有装备
                if (!data.isHaveWeapon)
                {
                    return;
                }
            }
            UIMagr.Instance.ShowPanel<TipsPanel>(E_UILayer.System, (panel) =>
            {
                //更新信息
                panel.ChangeTips(data.NowItemCallID);
                //更新位置
                panel.transform.position = data.transform.position;
                //防止异步加载
                if (isDrag)
                {
                    UIMagr.Instance.HidePanel<TipsPanel>();
                }
            });
        });
        //鼠标移出
        EventCenter.Instance.AddEventListener<ItemCall>(E_EventType.E_Bag_PointerExit, (data) =>
        {
            //隐藏tips面板
            UIMagr.Instance.HidePanel<TipsPanel>();
        });
        //拖动事件
        //开始拖动
        EventCenter.Instance.AddEventListener<ItemCall>(E_EventType.E_Bag_BeginDrag, (data) =>
        {
            isDrag = true;
            UIMagr.Instance.HidePanel<TipsPanel>();
            //记录当前拖动的格子
            nowDragItemCall = data;
            //如果是装备或者出售区 那么就不需要拖动
            if (nowDragItemCall.NowItemCallID < 0)
            {
                nowDragItemCall = null;
                return;
            }
            //通过缓存池创建一个新的物品图标
            nowItemImage = PoolMgr.Instance.GetObject("UI/ImageIcon").GetComponent<Image>();
            nowItemImage.sprite = data.imageIcon.sprite;
            //设置父对象 改变缩放大小
            nowItemImage.transform.SetParent(UIMagr.Instance.uiCanvas.transform);
            nowItemImage.transform.localScale = Vector3.one;
            //如果异步加载结束 直接进入缓存池
            if (!isDrag)
            {
                PoolMgr.Instance.PushObj(nowItemImage.gameObject);
                nowItemImage = null;
            }
        });
        //拖动中
        EventCenter.Instance.AddEventListener<BaseEventData>(E_EventType.E_Bag_Drag, (data) =>
        {
            //拖动中
            if (nowItemImage == null)
            {
                Debug.Log("nowItemImage为空");
                return;
            }
            Vector2 loaclPos;
            //将屏幕坐标转换为本地坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                nowItemImage.transform.parent.GetComponent<RectTransform>(),//得到父对象的RectTransform
                ((PointerEventData)data).position,//相对于鼠标位置
                ((PointerEventData)data).pressEventCamera,
                out loaclPos);
            nowItemImage.transform.localPosition = loaclPos;
        });
        //结束拖动
        EventCenter.Instance.AddEventListener<ItemCall>(E_EventType.E_Bag_EndDrag, (data) =>
        {
            ChangeItemCall();
            isDrag = false;
            //结束拖动置空
            nowDragItemCall = null;
            //拖动结束
            if (nowItemImage == null)
            {
                Debug.Log("nowItemImage为空");
                return;
            }
            PoolMgr.Instance.PushObj(nowItemImage.gameObject);
            nowItemImage = null;
        });
    }

    private void ChangeItemCall()
    {
        if (nowDragItemCall == null)
        {
            return;
        }
        //获取拖动的格子的物品数据
        ItemData itemData = playerData.ItemDataList[nowDragItemCall.NowItemCallID];
        //更换装备武器
        //进入的格子不为空 并且不是是背包中的格子
        if (nowInItemCall != null && nowInItemCall.NowItemCallID == -1)
        {
            //判断角色类型和物品类型是否匹配
            if (itemData.itemInfo.type == heroData.heroID)
            {
                //如果装备栏为空
                if (!nowInItemCall.isHaveWeapon)
                {
                    //直接装备
                    playerData.NowItemData = itemData;
                    //背包中移除
                    playerData.ItemDataList.Remove(itemData);
                    //添加到装备栏
                    //修改装备栏图片
                    nowInItemCall.imageIcon.sprite = nowDragItemCall.imageIcon.sprite;
                    //修改透明度
                    nowInItemCall.imageIcon.color = nowDragItemCall.imageIcon.color;
                    //设置是否装备武器
                    nowInItemCall.isHaveWeapon = true;
                    playerData.isWeapon = true;
                }
                //如果不为空 那么就交换武器
                else
                {
                    //交换图片
                    nowInItemCall.imageIcon.sprite = nowDragItemCall.imageIcon.sprite;
                    //交换背包中的物品数据 (析构交换)
                    (playerData.ItemDataList[nowDragItemCall.NowItemCallID], playerData.NowItemData) = (playerData.NowItemData, playerData.ItemDataList[nowDragItemCall.NowItemCallID]);
                    
                }
            }
        }
        //如果是拖动到丢弃的格子 那么便移除
        else if (nowInItemCall != null && nowInItemCall.NowItemCallID == -2)
        {
            Debug.Log("丢弃物品");
            //背包中移除
            playerData.ItemDataList.Remove(itemData);
        }
        //都不是就是交换物品
        else if (nowInItemCall != null)
        {
            //交换图片
            nowInItemCall.imageIcon.sprite = nowDragItemCall.imageIcon.sprite;
            //交换背包中的物品数据 (析构交换)
            (playerData.ItemDataList[nowDragItemCall.NowItemCallID], playerData.ItemDataList[nowInItemCall.NowItemCallID]) = (playerData.ItemDataList[nowInItemCall.NowItemCallID], playerData.ItemDataList[nowDragItemCall.NowItemCallID]);
        } 
        //更新背包
        UIMagr.Instance.ShowPanel<RolePanel>();
    }
    
    private BagMgr()
    {
        
    }
}
