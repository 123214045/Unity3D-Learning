using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public interface ISceneController
    {
        void LoadResources();
    }

    public interface UserAction
    {
        void MoveBoat();
        void ObjectIsClicked(GameObjects characterCtrl);
        void Restart();
    }

}