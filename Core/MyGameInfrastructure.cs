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

            var planeNode = _scene.CreateChild("Plane");
            planeNode.Scale = new Vector3(100, 1, 100);
            var planeObject = planeNode.CreateComponent<StaticModel>();
            planeObject.Model = ResourceCache.GetModel("Models/Plane.mdl");
            planeObject.SetMaterial(ResourceCache.GetMaterial("Materials/Grass.xml"));

            var lightNode = _scene.CreateChild("DirectionalLight");
            lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f));
            var light = lightNode.CreateComponent<Light>();
            //var physics = scene.CreateComponent<PhysicsWorld>();
            //physics.SetGravity(new Vector3(0, 0, 0));

            var cameraNode = _scene.CreateChild("camera");
            var camera = cameraNode.CreateComponent<Camera>();
            cameraNode.Position = (new Vector3(0.0f, 5.0f, 0.0f));
			
			Renderer.SetViewport(0, Viewport = new Viewport(Context, _scene, camera, null));
            


            Input.SetMouseVisible(true, false);
            
            
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
