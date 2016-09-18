using System.Threading.Tasks;
using Urho;
using Urho.Gui;
using Urho.Actions;
using System;

namespace MyGameInfrastructure
{
    public class MyGameInfrastructure : Application
	{
		const string CoinstFormat = "{0} coins";

		Scene _scene;
        private Camera _camera;
        private Vector3 _cameraDirection;
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
            Input.SubscribeToKeyDown(KeyDownHandler);
            CreateScene();
        }

        private void KeyDownHandler(KeyDownEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    AdjustCameraPosition(new Vector3(0, 0, 10));
                    break;
                case Key.S:
                    AdjustCameraPosition(new Vector3(0, 0, -10));
                    break;
                case Key.Esc:
                    Exit();
                    break;
            }
        }

        private void AdjustCameraPosition(Vector3 deltaPosition)
        {
            //_camera.Node.Position += deltaPosition;
            //cameraNode.GetComponent()

            //Viewport.Camera.Node.Position += deltaPosition;
            //Renderer.SetViewport(0, Viewport);
        }

        async void CreateScene()
		{
			_scene = new Scene();
			_scene.CreateComponent<Octree>();

            //CreateStandardPlane(new Vector3(-5, 25, 0), "Materials/MyTexture.xml");
            //CreateStandardPlane(new Vector3(25, 0, 25), "Materials/MyTextureGreen.xml");
            CreateBox(new Vector3(0, 0, 0), "Materials/MyTextureGreen.xml");

            SetupDirectionalLight(new Vector3(0, 1000, 0), new Vector3(0, -1, 0), 1.5f);
            SetupDirectionalLight(new Vector3(-1000, 0, 0), new Vector3(1, 0, 0), 0.5f);
            SetupDirectionalLight(new Vector3(0, 0, -1000), new Vector3(0, 0, 1), 0.5f);
            //CreatePointLight();

            //var physics = scene.CreateComponent<PhysicsWorld>();
            //physics.SetGravity(new Vector3(0, 0, 0));

            SetupCamera();

            Input.SetMouseVisible(true, false);
            
            await StartGameTask();
			
            Exit();
        }

        private void CreateBox(Vector3 position, string materialFileName)
        {
            var boxNode = _scene.CreateChild("Box_" + materialFileName);

            boxNode.Scale = new Vector3(100, 100, 100);
            boxNode.Position = position;

            var boxComponent = boxNode.CreateComponent<StaticModel>();
            boxComponent.Model = ResourceCache.GetModel("Models/box.mdl");
            boxComponent.SetMaterial(ResourceCache.GetMaterial(materialFileName));

        }

        private StaticModel CreateStandardPlane(Vector3 position, string materialFileName)
        {
            var planeNode = _scene.CreateChild("Plane_" + materialFileName);
            planeNode.Scale = new Vector3(100, 1, 100);
            planeNode.Position = position;
            var planeObject = planeNode.CreateComponent<StaticModel>();
            planeObject.Model = ResourceCache.GetModel("Models/Plane.mdl");
            planeObject.SetMaterial(ResourceCache.GetMaterial(materialFileName));

            return planeObject;
        }

        private void CreatePointLight()
        {
            var pointLightNode = _scene.CreateChild("PointLight");
            pointLightNode.Position = new Vector3(50, 2, 50);
            //pointLightNode.SetDirection(new Vector3(0, -1, 0));
            var pointLightComponent = pointLightNode.CreateComponent<Light>();
            pointLightComponent.Brightness = 2.0F;
            pointLightComponent.Color = Color.White;
            pointLightComponent.LightType = LightType.Point;
            pointLightComponent.FadeDistance = 5;
            pointLightComponent.Range = 25;
        }

        private void SetupCamera()
        {
            var cameraNode = _scene.CreateChild("camera");
            var _camera = cameraNode.CreateComponent<Camera>();
            //camera.Type
            cameraNode.Position = (new Vector3(0.0f, 0.0f, -200.0f));
            _cameraDirection = new Vector3(0, 0, 1.0f);
            
            _cameraDirection.Normalize();
            cameraNode.SetDirection(_cameraDirection);

            Renderer.SetViewport(0, Viewport = new Viewport(Context, _scene, _camera, null));
        }
        
        private void SetupDirectionalLight(Vector3 position, Vector3 direction, float brightness)
        {
            var directionalLightNode = _scene.CreateChild(string.Format("DirLight_{0}_{1}_{2}", position.X, position.Y, position.Z));
            directionalLightNode.Position = position;
            directionalLightNode.SetDirection(direction);
            var directionalLightComponent = directionalLightNode.CreateComponent<Light>();
            directionalLightComponent.Brightness = brightness;
            directionalLightComponent.LightType = LightType.Directional;
        }

        private Task StartGameTask()
        {
            _liveTask = new TaskCompletionSource<bool>();
            return _liveTask.Task;
        }

        protected override void OnUpdate(float timeStep)
        {
            const float speed = 0.5f;

            if (Input.GetMouseButtonDown(MouseButton.Left) || Input.NumTouches > 0)
            {
                _liveTask.SetResult(true);
            }


            var headingAngle = MyMath.Atan2(_cameraDirection.Z, _cameraDirection.X);
            var zDir = MyMath.Sin(headingAngle);
            var xDir = MyMath.Cos(headingAngle);
            var cameraPositionDelta = new Vector3();
            if(Input.GetKeyDown(Key.W))
            {
                cameraPositionDelta.X += speed * xDir;
                cameraPositionDelta.Z += speed * zDir;
            }
                

            if (Input.GetKeyDown(Key.S))
            {
                cameraPositionDelta.X -= speed * xDir;
                cameraPositionDelta.Z -= speed * zDir;
            }

            if (Input.GetKeyDown(Key.A))
            { 
                cameraPositionDelta.X -= speed * zDir;
                cameraPositionDelta.Z += speed * xDir;
            }

            if (Input.GetKeyDown(Key.D))
            { 
                cameraPositionDelta.X += speed * zDir;
                cameraPositionDelta.Z -= speed * xDir;
            }

            if (Input.GetKeyDown(Key.N1))
            {
                cameraPositionDelta.Y += speed;
            }

            if (Input.GetKeyDown(Key.N2))
            {
                cameraPositionDelta.Y -= speed;
            }

            if (Input.MouseMove.X != 0 || Input.MouseMove.Y != 0)
            {
                const float SCALING_FACTOR = 0.01f;
                float xzRotationDelta = -Input.MouseMove.X * SCALING_FACTOR; 
                float zyRotationDelta = -Input.MouseMove.Y * SCALING_FACTOR; 


                //x-axis rotation (looking up and down)
                _cameraDirection = Vector3.TransformNormal(_cameraDirection,
                    new Matrix4
                    {
                        Row0 = new Vector4(1, 0, 0, 0),
                        Row1 = new Vector4(0, MyMath.Cos(zyRotationDelta), -MyMath.Sin(zyRotationDelta), 0),
                        Row2 = new Vector4(0, MyMath.Sin(zyRotationDelta), MyMath.Cos(zyRotationDelta), 0),
                        Row3 = new Vector4(0, 0, 0, 1),
                    });

                //y-axis rotation (looking left and right)
                _cameraDirection = Vector3.TransformNormal(_cameraDirection,
                    new Matrix4
                    {
                        Row0 = new Vector4(MyMath.Cos(xzRotationDelta), 0, MyMath.Sin(xzRotationDelta), 0),
                        Row1 = new Vector4(0, 1, 0, 0),
                        Row2 = new Vector4(-MyMath.Sin(xzRotationDelta), 0, MyMath.Cos(xzRotationDelta), 0),
                        Row3 = new Vector4(0, 0, 0, 1),
                    });     

                //z-axis rotation (roll)
                //_cameraDirection = Vector3.TransformNormal(_cameraDirection,
                //   new Matrix4
                //   {
                //       Row0 = new Vector4(MyMath.Cos(xzRotationDelta), -MyMath.Sin(xzRotationDelta), 0, 0),
                //       Row1 = new Vector4(MyMath.Sin(xzRotationDelta), MyMath.Cos(xzRotationDelta), 0, 0),
                //       Row2 = new Vector4(0, 0, 1, 0),
                //       Row3 = new Vector4(0, 0, 0, 1),
                //   });

                Viewport.Camera.Node.SetDirection(_cameraDirection);
            }
                    

            //Vector3.Transform()

            //cameraPositionDelta.Normalize();
            Viewport.Camera.Node.Position += cameraPositionDelta;
            Renderer.SetViewport(0, Viewport);
            
            //base.OnUpdate(timeStep);
        }
        
	}
}
