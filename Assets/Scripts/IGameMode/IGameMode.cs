using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameMode
{
    bool CheckWinCondition();
    bool CheckGameOverCondition();
}
