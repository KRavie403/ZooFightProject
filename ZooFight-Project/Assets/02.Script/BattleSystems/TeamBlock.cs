using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBlock : BlockObject
{

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void SetTeam(Team team)
    {
        if(team == Team.NotSetting)
        {
            // 재설정할수있게 루틴만들기
            return;
        }
        myTeam = team;
    }

}
