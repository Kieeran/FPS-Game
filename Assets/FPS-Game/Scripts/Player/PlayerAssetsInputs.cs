using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace PlayerAssets
{
	public class PlayerAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool shoot;
		public bool reload;
		public bool openInventory;
		public bool openScoreboard;
		public bool interact;
		public bool hotkey1;
		public bool hotkey2;
		public bool hotkey3;
		public bool hotkey4;
		public bool hotkey5;
		public bool escapeUI;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}

		public void OnShoot(InputValue value)
		{
			ShootInput(value.isPressed);
		}

		public void OnReload(InputValue value)
		{
			ReloadInput(value.isPressed);
		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}

		public void OnOpenInventory(InputValue value)
		{
			OpenInventoryInput(value.isPressed);
		}

		public void OnOpenScoreboard(InputValue value)
		{
			OpenScoreboardInput(value.isPressed);
		}

		public void OnHotkey1(InputValue value)
		{
			Hotkey1Input(value.isPressed);
		}

		public void OnHotkey2(InputValue value)
		{
			Hotkey2Input(value.isPressed);
		}

		public void OnHotkey3(InputValue value)
		{
			Hotkey3Input(value.isPressed);
		}

		public void OnHotkey4(InputValue value)
		{
			Hotkey4Input(value.isPressed);
		}

		public void OnHotkey5(InputValue value)
		{
			Hotkey5Input(value.isPressed);
		}

		public void OnEscapeUI(InputValue value)
		{
			EscapeUIInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}

		public void ShootInput(bool newShootState)
		{
			shoot = newShootState;
		}

		public void ReloadInput(bool newReloadState)
		{
			reload = newReloadState;
		}

		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}

		public void OpenInventoryInput(bool newOpenInventoryState)
		{
			openInventory = newOpenInventoryState;
		}

		public void OpenScoreboardInput(bool newOpenScoreboardState)
		{
			openScoreboard = newOpenScoreboardState;
		}

		public void Hotkey1Input(bool newHotkey1State)
		{
			hotkey1 = newHotkey1State;
		}

		public void Hotkey2Input(bool newHotkey2State)
		{
			hotkey2 = newHotkey2State;
		}

		public void Hotkey3Input(bool newHotkey3State)
		{
			hotkey3 = newHotkey3State;
		}

		public void Hotkey4Input(bool newHotkey4State)
		{
			hotkey4 = newHotkey4State;
		}

		public void Hotkey5Input(bool newHotkey5State)
		{
			hotkey5 = newHotkey5State;
		}

		public void EscapeUIInput(bool newEscapeUIState)
		{
			escapeUI = newEscapeUIState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
}