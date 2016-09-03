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
            planeNode.Position = new Vector3(0, 0, 0);
            var planeObject = planeNode.CreateComponent<StaticModel>();
            planeObject.Model = ResourceCache.GetModel("Models/Plane.mdl");
            planeObject.SetMaterial(ResourceCache.GetMaterial("Materials/MyTexture.xml"));

            var planeNodeGreen = _scene.CreateChild("Plane");
            planeNodeGreen.Scale = new Vector3(100, 1, 100);
            planeNodeGreen.Position = new Vector3(25, -1, 25);
            var planeObjectGreen = planeNodeGreen.CreateComponent<StaticModel>();
            planeObjectGreen.Model = ResourceCache.GetModel("Models/Plane.mdl");
            planeObjectGreen.SetMaterial(ResourceCache.GetMaterial("Materials/MyTextureGreen.xml"));

            var ambientLightNode = _scene.CreateChild("AmbientLight");
            ambientLightNode.Position = new Vector3(-25, 50, -25);
            ambientLightNode.SetDirection(new Vector3(0, -1, 0));
            var ambientLightComponent = ambientLightNode.CreateComponent<Light>();
            ambientLightComponent.Brightness = 0.5F;
            ambientLightComponent.LightType = LightType.Directional;
            ambientLightComponent.FadeDistance = 20;

            var pointLightNode = _scene.CreateChild("PointLight");
            pointLightNode.Position = new Vector3(50, 2, 50);
            //pointLightNode.SetDirection(new Vector3(0, -1, 0));
            var pointLightComponent = pointLightNode.CreateComponent<Light>();
            pointLightComponent.Brightness = 2.0F;
            pointLightComponent.Color = Color.White;
            pointLightComponent.LightType = LightType.Point;
            pointLightComponent.FadeDistance = 5;
            pointLightComponent.Range = 25;

            //var physics = scene.CreateComponent<PhysicsWorld>();
            //physics.SetGravity(new Vector3(0, 0, 0));

            var cameraNode = _scene.CreateChild("camera");
            var camera = cameraNode.CreateComponent<Camera>();
            cameraNode.Position = (new Vector3(50.0f, 100.0f, 0.0f));
            var cameraDirection = new Vector3(0, -1, 0.5f);
            cameraDirection.Normalize();
            cameraNode.SetDirection(cameraDirection);
			
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
