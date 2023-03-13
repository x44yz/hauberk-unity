using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

/// Name:           Emergency Exit
/// Description:    Breaks the main thread out of infinite loops
/// Author:         Aevus I
/// Version:        1.1.0

namespace Aevus
{
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
#endif
	public static class EmergencyExit
	{
		static Thread mainThread, emergencyThread;
		
		static EmergencyExit()
		{
			Start();
		}

#if !UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod]
#endif
		static void Start()
		{
			Application.logMessageReceived -= CatchThreadAbort;
			Application.logMessageReceived += CatchThreadAbort;
		
			ResetAbortThreadFlag();
			SpawnEmergencyThreadIfItDoesNotAlreadyExist();
		}

		private static void CatchThreadAbort(string condition, string stackTrace, UnityEngine.LogType type)
		{
			if (type == UnityEngine.LogType.Exception && condition == "ThreadAbortException")
			{
				ResetAbortThreadFlag();
			}
		}

		public static void ResetAbortThreadFlag()
		{
			if ((Thread.CurrentThread.ThreadState & (ThreadState.AbortRequested | ThreadState.Aborted)) != 0)
			{
				Thread.ResetAbort();
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPaused = true;
#endif
				
				// custom code here... 
				
			}
		}

		static void SpawnEmergencyThreadIfItDoesNotAlreadyExist()
		{
			if (mainThread == null)
				mainThread = Thread.CurrentThread;

			if (emergencyThread == null || !emergencyThread.IsAlive)
			{
				emergencyThread = new Thread(EmergencyTerminationThread);
				emergencyThread.Name = "Aevus Emergency Exit Thread";
				emergencyThread.Start();
			}
		}

		// this function will run as a seperate thread in the background and monitor
		// the keyboard for the abort keycode. then it will throw the Abort exception 
		// to the main thread, which will then hopefully cause the main thread to 
		// break out of any infinite loops and then get cought by the unity main thread.
		static void EmergencyTerminationThread()
		{
			while (true)
			{
				//log to the console that this emergency thread is active, if ctrl and f9 are pressed at the same time
				if (ShowEmergencyThreadActivity())
					UnityEngine.Debug.Log("Aevus emergencyThreadId " + Thread.CurrentThread.ManagedThreadId);

				if (EmergencyStopCode())
				{
					try
					{
						mainThread.Abort();
					}
					catch (Exception e)
					{
						UnityEngine.Debug.LogError(e);
					}
				}
				Thread.Sleep(100); // use less resources
			}
		}

		// todo: add support for mac and linux. 
		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState(int keycode);

		static bool lastESC = false;

		static bool EmergencyStopCode()
		{
			// https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
			// are shift, ctrl and Q currently pressed?
			bool shift = GetAsyncKeyState(0x10) < 0;
			bool ctrl = GetAsyncKeyState(0x11) < 0;
			bool q = GetAsyncKeyState(0x51) < 0;
			bool e = GetAsyncKeyState(0x45) < 0;

			// the bool "activate" will only be active once while ctrl shift and q are being held down
			bool pressed = shift && ctrl && q;
			bool activate = pressed && !lastESC;
			lastESC = pressed;

			// return true when activate is set or when ctrl shift and e are held down.
			return activate || (ctrl && shift && e);
		}

		static bool ShowEmergencyThreadActivity()
		{
			// https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
			// are shift, ctrl and H currently pressed?
			bool shift = GetAsyncKeyState(0x10) < 0;
			bool ctrl = GetAsyncKeyState(0x11) < 0;
			bool h = GetAsyncKeyState(0x48) < 0;

			return shift && ctrl && h;
		}
	}
}
