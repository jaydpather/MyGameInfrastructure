using System.Threading.Tasks;
using Urho;
using Urho.Gui;
using Urho.Actions;

namespace SamplyGame
{
    public class MyGameInfrastructure : Application
	{
		const string CoinstFormat = "{0} coins";

		Scene _scene;
        //private bool _isMouseDown = false;
        private TaskCompletionSource<bool> _liveTask;

		public Viewport Viewport { get; private set; }

		public MyGameInfrastructure() 
            : base(new ApplicationOptions(assetsFolder: "Data")
                {
                    Height = 736,
                    Width = 414,
                    Orientation = ApplicationOptions.OrientationType.Portrait
                }
            )
        {
        }

		protected override void Start()
		{
			base.Start();
            Input.SubscribeToKeyDown(e => { if (e.Key == Key.Esc) Exit(); });
            CreateScene();
        }
        
		async void CreateScene()
		{
			_scene = new Scene();
			_scene.CreateComponent<Octree>();

			//var physics = scene.CreateComponent<PhysicsWorld>();
			//physics.SetGravity(new Vector3(0, 0, 0));
            
			var cameraNode = _scene.CreateChild();
			cameraNode.Position = (new Vector3(0.0f, 0.0f, -10.0f));
			cameraNode.CreateComponent<Camera>();
			
			Renderer.SetViewport(0, Viewport = new Viewport(Context, _scene, cameraNode.GetComponent<Camera>(), null));
            
            Input.SetMouseVisible(true, false);
            
            var lightNode1 = _scene.CreateChild();
            lightNode1.Position = new Vector3(0, 0, -40);
            lightNode1.AddComponent(new Light { Range = 120, Brightness = 1.5f });
            
            await StartGameTask();
			
            Exit();
        }

        private Task StartGameTask()
        {
            _liveTask = new TaskCompletionSource<bool>();
            return _liveTask.Task;
        }

        protected override void OnUpdate(float timeStep)
        {
            if (Input.GetMouseButtonDown(MouseButton.Left) || Input.NumTouches > 0)
            {
                _liveTask.SetResult(true);
            }
            //base.OnUpdate(timeStep);
        }
        
	}
}
