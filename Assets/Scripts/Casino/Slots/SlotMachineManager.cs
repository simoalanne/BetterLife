using System.Collections;
using System.Linq;
using Helpers;
using UnityEngine;

namespace Casino.Slots
{
    public class SlotMachineManager : MonoBehaviour
    {
        [SerializeField] private Reel[] reels;

        [SerializeField] private Animator handleAnimation;

        [SerializeField] private Animator slotMachineAnimation;

        [SerializeField] private float howLongToSpin = 3f;

        [SerializeField] private float reelSpinSpeed = 5f;

        private bool _roundUnderway;

        private void OnMouseDown()
        {
            if (_roundUnderway || !Services.SlotMoneyHandler.PlaceBet(Services.BetSizeManager.CurrentBetSize)) return;
            _roundUnderway = true;
            StartCoroutine(PullHandle());
        }

        private IEnumerator PullHandle()
        {
            slotMachineAnimation.enabled = false;
            handleAnimation.SetBool("playSpin", true);
            yield return new WaitForSeconds(0.3f);
            handleAnimation.SetBool("playSpin", false);

            foreach (var reel in reels)
            {
                reel.StartSpinning(reelSpinSpeed);
            }

            yield return new WaitForSeconds(howLongToSpin);

            foreach (var reel in reels)
            {
                reel.StopSpinning();
                yield return new WaitUntil(() => reel.ReelState == ReelState.Stopped);
            }

            CheckResults();
        }

        private void CheckResults()
        {
            var winningAmount = Services.SlotMoneyHandler.CreditWinnings(reels.Select(r => r.StoppedSlot).ToList());
            slotMachineAnimation.enabled = winningAmount > 0;
            _roundUnderway = false;
        }
    }
}
