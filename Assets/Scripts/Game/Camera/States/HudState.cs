using Game.Camera.GUIs;
using Game.ItemSystem;
using Game.ItemSystem.Weapon;
using Game.Player;
using UnityEngine;

namespace Game.Camera.States
{
    public class HudState : GuiBaseStateWithPayload<PlayerController>
    {
        private HudGui _hudGui;
        private UsableItem _item;
        private PlayerController _playerController;

        public HudState(GuiFactory guiFactory) : base(guiFactory) { }
        
        protected override void OnEnter(PlayerController playerController)
        {
            _hudGui = GuiFactory.CreateGUI<HudGui>();

            _playerController = playerController;

            _playerController.OnItemChange += ItemChange;
            _playerController.OnHealthChange += UpdateHealth;
            _playerController.OnDeath += OnDeath;
        }

        protected override void OnExit()
        {
            _playerController.OnHealthChange -= UpdateHealth;
            _playerController.OnDeath -= OnDeath;

            Object.Destroy(_hudGui.gameObject);
        }
        
        private void ItemChange(UsableItem item)
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
        }

        private void OnDeath()
        {
            StateMachine.SetState<DeathGuiState, PlayerController>(_playerController);
        }
        
        private void WeaponReload(float reloadTime)
        {
            _hudGui.currentReloadTime = reloadTime;
            _hudGui.ReloadIndicator.gameObject.SetActive(true);
            _hudGui.ReloadIndicator.fillAmount = 0f;
        }
        
        private void UpdateAmmunitionDisplay(int magazineSize, int bulletsLeft, int bulletPerTap)
        {
            if (_hudGui.AmmunitionDisplay != null)
            {
                _hudGui.AmmunitionDisplay.text =
                    $"{bulletsLeft / bulletPerTap} / {magazineSize / bulletPerTap}";
            }
        }
        
        void UpdateHealth(float currentHealth, float maxHealth)
        {
            _hudGui.HealthBarImage.fillAmount = currentHealth / maxHealth;
            _hudGui.HealthBarGameObject.SetActive(maxHealth - currentHealth != 0);
        }
    }
}