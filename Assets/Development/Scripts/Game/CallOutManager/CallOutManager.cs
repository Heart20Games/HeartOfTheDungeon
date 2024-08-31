using UnityEngine;
using System.Collections.Generic;

public class CallOutManager : MonoBehaviour
{
    public static CallOutManager instance;

    [SerializeField] private Party party;

    private List<HotD.Body.Character> partyMembers = new List<HotD.Body.Character>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        ChangePartyMembers();
    }

    public void ChangePartyMembers()
    {
        if(partyMembers.Count > 0)
        {
            partyMembers.Clear();
        }

        for(int i = 0; i < party.members.Count; i++)
        {
            if (party.members[i] != party.leader)
            {
                partyMembers.Add(party.members[i]);
            }
        }
    }

    public void PlayPartyMemeberCallOut(int callIndex)
    {
        int rand = Random.Range(0, 2);

        switch(rand)
        {
            case 0:
                partyMembers[0].GetComponent<CallOutBarks>().PlayBark(callIndex);
                break;
            case 1:
                partyMembers[1].GetComponent<CallOutBarks>().PlayBark(callIndex);
                break;
        }
    }
}