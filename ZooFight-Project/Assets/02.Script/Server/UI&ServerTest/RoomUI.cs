using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd.Tcp;
using Battlehub.Dispatcher;
using BackEnd;

public partial class MainMenuManager : MonoBehaviour
{
    //public GameObject readyRoomObject;
    //public GameObject friendListParent;
    //public GameObject readyUserListParent;
    //public GameObject friendPrefab;
    //public GameObject friendEmptyObject;

    private GameObject readyRoomObject;
    private GameObject friendListParent;
    private GameObject readyUserListParent;
    private GameObject friendPrefab;
    private GameObject friendEmptyObject;

    private List<string> readyUserList = null;
    public void OpenRoomUI()
    {
        // 매치 서버에 대기방 생성 요청
        if (BackEndMatchManager.GetInstance().CreateMatchRoom() == true)
        {
            SetLoadingObjectActive(true);   
        }   
    }

    public void CreateRoomResult(bool isSuccess, List<MatchMakingUserInfo> userList = null)
    {
        // 대기 방 생성에 성공 시 대기방 UI를 활성화 시키고,
        // 친구목록을 조회
        if (isSuccess == true)
        {
            readyRoomObject.SetActive(true);
            if (userList == null)
            {
                SetReadyUserList(BackEndServerManager.GetInstance().myNickName);
            }
            else
            {
                SetReadyUserList(userList);
            }
        }
        // 대기 방 생성에 실패 시 에러를 띄움
        else
        {
            SetLoadingObjectActive(false);
#if UNITY_EDITOR || DEBUG
            Debug.Log("대기방 생성에 실패했습니다.\n\n잠시 후 다시 시도해주세요.");
#endif
        }
    }

    public void LeaveReadyRoom()
    {
        BackEndMatchManager.GetInstance().LeaveMatchLoom();
        // readyRoomObject.SetActive(false);
    }

    public void CloseRoomUIOnly()
    {
        readyRoomObject.SetActive(false);
    }

    public void RequestMatch()
    {
        //if (loadingObject.activeSelf || recordObject.activeSelf || errorObject.activeSelf || requestProgressObject.activeSelf || matchDoneObject.activeSelf)
        //{
        //    return;
        //}

        foreach (var tab in matchInfotabList)
        {
            if (tab.IsOn() == true)
            {
                BackEndMatchManager.GetInstance().RequestMatchMaking(tab.index);
                return;
            }
        }

        Debug.Log("활성화된 탭이 존재하지 않습니다.");
    }

    // 친구 목록 설정
    //public void SetFriendList()
    //{
    //    ClearFriendList();
    //    BackEndServerManager.GetInstance().GetFriendList((bool result, List<Friend> friendList) =>
    //    {
    //        Dispatcher.Current.BeginInvoke(() =>
    //        {
    //            SetLoadingObjectActive(false);
    //            if (result == false)
    //            {
    //                friendEmptyObject.SetActive(true);
    //                return;
    //            }

    //            if (friendList == null)
    //            {
    //                friendEmptyObject.SetActive(true);
    //                return;
    //            }

    //            if (friendList.Count <= 0)
    //            {
    //                friendEmptyObject.SetActive(true);
    //                return;
    //            }

    //            foreach (var tmp in friendList)
    //            {
    //                InsertFriendPrefab(tmp.nickName);
    //            }

    //            friendEmptyObject.SetActive(false);
    //        });
    //    });
    //}

    private void ClearFriendList()
    {
        var parent = friendListParent.transform;

        while (parent.childCount > 0)
        {
            var child = parent.GetChild(0);
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    private void InsertFriendPrefab(string nickName)
    {
        GameObject friend = GameObject.Instantiate(friendPrefab, Vector3.zero, Quaternion.identity, friendListParent.transform);
        friend.GetComponentInChildren<Text>().text = nickName;
    }

    // 대기방 인원 설정
    public void SetReadyUserList(List<MatchMakingUserInfo> userList)
    {
        ClearReadyUserList();

        if (userList == null)
        {
            Debug.LogError("ready user list is null");
            return;
        }
        if (userList.Count <= 0)
        {
            Debug.LogError("ready user list is empty");
            return;
        }

        foreach (var user in userList)
        {
            InsertReadyUserPrefab(user.m_nickName);
        }
    }

    public void SetReadyUserList(string nickName)
    {
        ClearReadyUserList();

        if (string.IsNullOrEmpty(nickName))
        {
            Debug.LogError("ready user list is empty");
            return;
        }

        InsertReadyUserPrefab(nickName);
    }

    public void InsertReadyUserPrefab(string nickName)
    {
        if (readyUserList == null)
        {
            return;
        }

        if (readyUserList.Contains(nickName))
        {
            return;
        }

        GameObject friend = GameObject.Instantiate(friendPrefab, Vector3.zero, Quaternion.identity, readyUserListParent.transform);
        friend.GetComponentInChildren<Text>().text = nickName;

        readyUserList.Add(nickName);
    }

    public void DeleteReadyUserPrefab(string nickName)
    {
        if (readyUserList == null)
        {
            return;
        }

        if (readyUserList.Contains(nickName) == false)
        {
            return;
        }

        var parent = readyUserListParent.transform;

        for (int i = 0; i < readyUserList.Count; ++i)
        {
            if (nickName.Equals(readyUserList[i]) == false)
            {
                continue;
            }
            var child = parent.GetChild(i);
            if (child == null)
            {
                continue;
            }
            GameObject.DestroyImmediate(child.gameObject);
        }

        readyUserList.Remove(nickName);
    }

    private void ClearReadyUserList()
    {
        readyUserList = new List<string>();
        var parent = readyUserListParent.transform;

        while (parent.childCount > 0)
        {
            var child = parent.GetChild(0);
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
}

