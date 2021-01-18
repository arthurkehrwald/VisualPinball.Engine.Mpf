// Visual Pinball Engine
// Copyright (C) 2021 freezy and VPE Team
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using UnityEngine;
using VisualPinball.Engine.Game.Engines;
using VisualPinball.Unity;

namespace VisualPinball.Engine.Mpf.Unity
{
	[Serializable]
	[DisallowMultipleComponent]
	[AddComponentMenu("Visual Pinball/Game Logic Engine/Mission Pinball Framework")]
	public class MpfGamelogicEngine : MonoBehaviour, IGamelogicEngine,
		IGamelogicEngineWithSwitches, IGamelogicEngineWithCoils, IGamelogicEngineWithLamps
	{
		public string Name { get; } = "Mission Pinball Framework";

		public GamelogicEngineSwitch[] AvailableSwitches { get; } = new GamelogicEngineSwitch[0];
		public GamelogicEngineCoil[] AvailableCoils { get; } = new GamelogicEngineCoil[0];
		public GamelogicEngineLamp[] AvailableLamps { get; } = new GamelogicEngineLamp[0];

		public event EventHandler<LampEventArgs> OnLampChanged;
		public event EventHandler<LampsEventArgs> OnLampsChanged;
		public event EventHandler<LampColorEventArgs> OnLampColorChanged;
		public event EventHandler<CoilEventArgs> OnCoilChanged;

		public void OnInit(Player player, TableApi tableApi, BallManager ballManager)
		{
		}

		public void Switch(string id, bool isClosed)
		{
		}

		public void OnUpdate()
		{
		}

		public void OnDestroy()
		{
		}

		public async void RefreshFromMpf()
		{
			using var mpfApi = new MpfApi(@"../../VisualPinball.Engine.Mpf/VisualPinball.Engine.Mpf/machine");
			await mpfApi.Launch();

			mpfApi.Start(new Dictionary<string, bool> {
				{"sw_11", false},
			});

			var descr = await mpfApi.GetMachineDescription();

			Debug.Log(descr);
		}
	}
}
