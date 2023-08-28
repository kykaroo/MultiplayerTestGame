using Game.Camera.GUIs;
using UnityEngine;

namespace Game.Camera.States
{
    using Player = Game.Player.Player;
    public class HudState : GuiBaseStateWithPayload<Player>
    {
        private HudGui _hudGui;
        // private UsableItem _item;
        private Player _player;

        public HudState(GuiFactory guiFactory) : base(guiFactory) { }
        
        protected override void OnEnter(Player player)
        {
            _hudGui = GuiFactory.CreateGUI<HudGui>();

            _player = player;

            // _playerController.OnItemChange += ItemChange;
            _player.Health.OnHealthChange += UpdateHealth;
            _player.Health.OnDeath += OnDeath;
            _player.Input.OnReload += WeaponReload;
            _player.Input.OnAmmunitionUpdate += UpdateAmmunitionDisplay;
            _player.Input.OnSpeedUpdate += UpdateSpeedDisplay;
        }

        protected override void OnExit()
        {
            _player.Health.OnHealthChange -= UpdateHealth;
            _player.Health.OnDeath -= OnDeath;
            _player.Input.OnReload -= WeaponReload;
            _player.Input.OnAmmunitionUpdate -= UpdateAmmunitionDisplay;
            _player.Input.OnSpeedUpdate -= UpdateSpeedDisplay;

            Object.Destroy(_hudGui.gameObject);
        }
        
        /*private void ItemChange(UsableItem item)
        {
            switch (_item)
            {
                case BulletWeapon weapon:
                    weapon.OnReload -= WeaponReload;
                    weapon.OnAmmunitionChange -= UpdateAmmunitionDisplay;
                    break;
            }
            
            _item = item;
            
            switch (item)
            {
                case BulletWeapon weapon:
                    weapon.OnReload += WeaponReload;
                    weapon.OnAmmunitionChange += UpdateAmmunitionDisplay;
                    break;
            }
        }*/

        private void OnDeath(Vector3 vector3)
        {
            StateMachine.SetState<DeathGuiState, Player>(_player);
        }
        
        private void WeaponReload(float reloadTime)
        {
            _hudGui.currentReloadTime = reloadTime;
            _hudGui.ReloadIndicator.gameObject.SetActive(true);
            _hudGui.ReloadIndicator.fillAmount = 0f;
        }

        private void UpdateAmmunitionDisplay(int ammoLeft, int maxAmmo)
        {
            if (_hudGui.AmmunitionDisplay != null)
            {
                _hudGui.AmmunitionDisplay.text =
                    $"{ammoLeft} / {maxAmmo}";
            }
        }
        
        void UpdateHealth(float maxHealth, float currentHealth)
        {
            _hudGui.HealthBarImage.fillAmount = currentHealth / maxHealth;
            _hudGui.HealthBarGameObject.SetActive(maxHealth - currentHealth != 0);
        }

        private void UpdateSpeedDisplay(string speed)
        {
            _hudGui.SpeedDisplay.text = speed;
        }
    }
}