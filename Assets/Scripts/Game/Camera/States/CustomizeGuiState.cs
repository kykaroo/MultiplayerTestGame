using Game.Camera.GUIs;
using UnityEngine;

namespace Game.Camera.States
{
    public class CustomizeGuiState : GuiStateBase
    {
        private CustomizeGui _customizeGui;
        private UnityEngine.Camera _camera;

        public CustomizeGuiState(GuiFactory guiFactory, UnityEngine.Camera camera) : base(guiFactory)
        {
            _camera = camera;
        }
        
        protected override void OnEnter()
        {
            _customizeGui = GuiFactory.CreateGUI<CustomizeGui>();

            _customizeGui.GetComponent<Canvas>().worldCamera = _camera;
            _customizeGui.GetComponent<Canvas>().planeDistance = 10;    
            _customizeGui.OnBackButtonClick += Back;
            _camera.gameObject.SetActive(true);
        }

        protected override void OnExit()
        {
            _customizeGui.OnBackButtonClick -= Back;
            _camera.gameObject.SetActive(false);
            
            Object.Destroy(_customizeGui.gameObject);
        }
        
        private void Back()
        {
            StateMachine.SetState<PreGameState>();
        }
    }
}