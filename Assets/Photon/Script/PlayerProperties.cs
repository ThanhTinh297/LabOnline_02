using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    private PlayerInfo _playerInfo { get; set; }

    [SerializeField] Image healthSlider;
    [SerializeField] Image manaSlider;
    [SerializeField] TextMeshProUGUI scoreText;

    [SerializeField] GameObject fireBallPrefab;
    [SerializeField] GameObject Skill;
    [SerializeField] Transform fireBallSpawnPoint;
    private void OnHealthChanged()
    {
        healthSlider.fillAmount = _playerInfo.health / 100f;
        manaSlider.fillAmount = _playerInfo.mana / 60f;
        scoreText.text=$"{_playerInfo.score}";
        
        if (_playerInfo.health <= 0)
        {
            Debug.Log($"Player {Runner.LocalPlayer.PlayerId} is dead");
            Die();
        }
    }


    void Start()
    {
        if (HasStateAuthority)
        {
            _playerInfo = new PlayerInfo
            {
                health = 100,
                mana = 60,
                score=0
            };
        }
    }

    void Update()
    {
        if (HasStateAuthority)
        {
            if (Input.GetKeyDown(KeyCode.E) && _playerInfo.mana > 0)
            {
                Runner.Spawn(fireBallPrefab, fireBallSpawnPoint.position,
                    fireBallSpawnPoint.rotation, Runner.LocalPlayer, (runner, obj) =>
                    {
                        var fireBall = obj.GetComponent<FireBall>();
                        fireBall.Owner = this;
                        fireBall.Setup(fireBallSpawnPoint.forward.normalized);
                    });

                var currentHealth = _playerInfo.health;
                var currentMana = _playerInfo.mana;
                var currentScore=_playerInfo.score;
                _playerInfo = new PlayerInfo
                {
                    health = currentHealth,
                    mana = currentMana - 10,
                    score = currentScore
                };
            }
            if (Input.GetKeyDown(KeyCode.R) && _playerInfo.mana > 0)
            {
                Runner.Spawn(Skill, fireBallSpawnPoint.position,
                    fireBallSpawnPoint.rotation, Runner.LocalPlayer, (runner, obj) =>
                    {
                        var fireBall = obj.GetComponent<FireBall>();
                        fireBall.Setup(fireBallSpawnPoint.forward.normalized);
                    });

                var currentHealth = _playerInfo.health;
                var currentMana = _playerInfo.mana;
                _playerInfo = new PlayerInfo
                {
                    health = currentHealth,
                    mana = currentMana - 30
                };
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (HasStateAuthority)
        {
            var currentHealth = _playerInfo.health;
            var currentMana = _playerInfo.mana;
            var currentScore = _playerInfo.score;
            _playerInfo = new PlayerInfo
            {
                health = currentHealth - damage,
                mana = currentMana,
                score=currentScore
                
            };
        }
    }
    public void IncreaseScore(int score)
    {
        if (HasStateAuthority)
        {
            var currentHealth = _playerInfo.health;
            var currentMana = _playerInfo.mana;
            var currentScore = _playerInfo.score;
            _playerInfo = new PlayerInfo
            {
                health = currentHealth,
                mana = currentMana,
                score = currentScore+score

            };
        }
    }

    private void Die()
    {
        _playerInfo = new PlayerInfo
        {
            health = 0,
            mana = 0
        };
        GetComponent<Animator>().SetBool("IsDead", true);
        GetComponent<CharacterController>().enabled = false;
    }

    public void ManaPoolFill()
    {
        if (HasStateAuthority)
        {
            var currentHealth = _playerInfo.health;
            var currentMana = _playerInfo.mana;
            var currentScore=_playerInfo.score;
            _playerInfo = new PlayerInfo
            {
                health = currentHealth,
                mana = currentMana + 10,
                score = currentScore
            };
        }
    }
}