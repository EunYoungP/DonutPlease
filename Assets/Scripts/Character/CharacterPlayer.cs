using UnityEngine;

namespace DonutPlease.Game.Character
{
    public class CharacterPlayer : CharacterBase
    {
        private float moveSpeed = 5f;

        public bl_Joystick Joystick;

        public CharacterPlayer()
        {
            Joystick = GameManager.GetGameManager._canvas.GetComponentInChildren<bl_Joystick>();
        }

        // Move
        private void Update()
        {
            Joystick = GameManager.GetGameManager._canvas.GetComponentInChildren<bl_Joystick>();

            float v = Joystick.Vertical;
            float h = Joystick.Horizontal;

            Vector3 dir = new Vector3(h, 0, v);
            transform.Translate(dir * moveSpeed * Time.deltaTime);
        }
    }
}

