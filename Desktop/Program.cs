using Urho;
using Urho.Desktop;
using MyGameInfrastructure;

namespace SamplyGame.Desktop
{
	class Program
	{
		static void Main(string[] args)
		{
			DesktopUrhoInitializer.AssetsDirectory = @"../../Assets";
			new MyGameInfrastructure.MyGameInfrastructure().Run();
		}
	}
}
