using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Scenes
{
	/// <summary>
	/// IManagers are like IComponents, but they don't run on GameObjects, but on the scene itself
	/// </summary>
	public interface IManager
	{
		void OnLoad();
	}
	/// <summary>
	/// An IManager for which Update() is called every frame
	/// </summary>
	public interface IUpdatableManager:IManager
	{
		void Update();
	}
}
