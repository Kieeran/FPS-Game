// Provided by MON Studios LLC as part of the "Button Effect Kit By MONS" Asset Pack.
// Redistribution or resale of this code, in whole or in part, is not allowed without permission from MON Studios LLC.
// Please feel free to use and modify this script within your own projects!

using UnityEngine;
using System.Collections;

namespace MONStudiosLLC.ButtonEffects
{
    public class ButtonEffects : MonoBehaviour
    {
        private float animationDuration = 0.1f;
        private bool isAnimating = false;

        // --- SCALE EFFECTS ---
        public void Press01ScaleUp(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Scale(buttonRectTransform, Vector3.one, Vector3.one * 1.5f));
        }

        public void Press02ScaleDown(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Scale(buttonRectTransform, Vector3.one, Vector3.one * 0.6f));
        }

        public void Press03ShrinkTemporarily(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(TemporaryShrink(buttonRectTransform, Vector3.one * 0.25f));
        }

        public void Press04ExpandHorizontally(RectTransform buttonRectTransform)
        {
            if (!isAnimating) buttonRectTransform.localScale = new Vector3(1.3f, buttonRectTransform.localScale.y, 1f);
        }

        public void Press05ExpandVertically(RectTransform buttonRectTransform)
        {
            if (!isAnimating) buttonRectTransform.localScale = new Vector3(buttonRectTransform.localScale.x, 1.3f, 1f);
        }

        public void Press06ContractHorizontally(RectTransform buttonRectTransform)
        {
            if (!isAnimating) buttonRectTransform.localScale = new Vector3(0.7f, buttonRectTransform.localScale.y, 1f);
        }

        public void Press07ContractVertically(RectTransform buttonRectTransform)
        {
            if (!isAnimating) buttonRectTransform.localScale = new Vector3(buttonRectTransform.localScale.x, 0.7f, 1f);
        }

        public void Press08Squash(RectTransform buttonRectTransform)
        {
            if (!isAnimating) buttonRectTransform.localScale = new Vector3(1.5f, 0.5f, 1f);
        }

        public void Press09Stretch(RectTransform buttonRectTransform)
        {
            if (!isAnimating) buttonRectTransform.localScale = new Vector3(0.5f, 1.5f, 1f);
        }

        public void Press10StretchHorizontally(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Scale(buttonRectTransform, Vector3.one, new Vector3(1.5f, 1f, 1f)));
        }

        public void Press11StretchVertically(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Scale(buttonRectTransform, Vector3.one, new Vector3(1f, 1.5f, 1f)));
        }

        public void Press12ScaleUpAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ScaleAndBack(buttonRectTransform, Vector3.one, Vector3.one * 1.5f));
        }

        public void Press13ScaleDownAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ScaleAndBack(buttonRectTransform, Vector3.one, Vector3.one * 0.6f));
        }

        public void Press14StretchHorizontallyAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ScaleAndBack(buttonRectTransform, Vector3.one, new Vector3(1.5f, 1f, 1f)));
        }

        public void Press15StretchVerticallyAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ScaleAndBack(buttonRectTransform, Vector3.one, new Vector3(1f, 1.5f, 1f)));
        }

        public void Press16StretchHorizontallyAndBackRight(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ScaleAndBack(buttonRectTransform, Vector3.one, new Vector3(-1.5f, -1f, -1f)));
        }

        public void Press17StretchVerticallyAndBackLeft(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ScaleAndBack(buttonRectTransform, Vector3.one, new Vector3(-1f, -1.5f, -1f)));
        }

        // --- BOUNCE AND PULSE EFFECTS ---
        public void Press18BounceUp(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(BounceEffect(buttonRectTransform, Vector3.one, Vector3.one * 1.4f));
        }

        public void Press19BounceDown(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(BounceEffect(buttonRectTransform, Vector3.one, Vector3.one * 0.8f));
        }

        public void Press20Bounce(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(BounceEffect(buttonRectTransform, Vector3.one, Vector3.one * 0.5f));
        }

        public void Press21Ripple(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(PulseEffect(buttonRectTransform, Vector3.one, Vector3.one * 1.5f));
        }

        public void Press22Pop(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(PulseEffect(buttonRectTransform, Vector3.one * 0.8f, Vector3.one * 1.5f));
        }

        public void Press23QuickPulse(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(PulseEffect(buttonRectTransform, Vector3.one * 0.8f, Vector3.one * 1.3f));
        }

        public void Press24RippleAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(PulseAndBack(buttonRectTransform, Vector3.one, Vector3.one * 1.2f));
        }

        public void Press25PopAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(PulseAndBack(buttonRectTransform, Vector3.one * 0.8f, Vector3.one * 1.5f));
        }

        // --- ROTATION EFFECTS ---
        public void Press26RotateScalePulse(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateScalePulseEffect(buttonRectTransform));
        }

        public void Press27RotateClockwise(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Rotate(buttonRectTransform, 0f, -30f));
        }

        public void Press28RotateCounterClockwise(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Rotate(buttonRectTransform, 0f, 30f));
        }

        public void Press29TiltUp(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Rotate(buttonRectTransform, 0f, 15f));
        }

        public void Press30TiltDown(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Rotate(buttonRectTransform, 0f, -15f));
        }

        public void Press31Wobble(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Rotate(buttonRectTransform, Random.Range(-30f, 30f), 0f));
        }

        public void Press32Twist(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(TwistEffect(buttonRectTransform));
        }

        public void Press33SpinRight(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(SpinEffect(buttonRectTransform, 360f, 0.5f));
        }

        public void Press34SpinLeft(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(SpinEffect(buttonRectTransform, -360f, 0.5f));
        }

        public void Press35RotateBackAndForth(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateBackAndForth(buttonRectTransform, 15f));
        }

        public void Press36RotateClockwiseAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateAndBack(buttonRectTransform, 0f, -30f));
        }

        public void Press37RotateCounterClockwiseAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateAndBack(buttonRectTransform, 0f, 30f));
        }

        public void Press38TiltUpAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateAndBack(buttonRectTransform, 0f, 15f));
        }

        public void Press39TiltDownAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateAndBack(buttonRectTransform, 0f, -15f));
        }

        public void Press40WobbleAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateAndBack(buttonRectTransform, Random.Range(-15f, 15f), 0f));
        }

        public void Press41RotateAndScaleAndBackRight(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateAndScaleAndBack(buttonRectTransform, 0f, 45f, Vector3.one, Vector3.one * 1.2f));
        }

        public void Press42RotateAndScaleAndBackLeft(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateAndScaleAndBack(buttonRectTransform, 0f, -45f, Vector3.one, Vector3.one * 1.2f));
        }

        // --- MOVE EFFECTS ---
        public void Press43MoveUp(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Move(buttonRectTransform, buttonRectTransform.anchoredPosition, buttonRectTransform.anchoredPosition + new Vector2(0, 20f)));
        }

        public void Press44MoveDown(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Move(buttonRectTransform, buttonRectTransform.anchoredPosition, buttonRectTransform.anchoredPosition - new Vector2(0, 20f)));
        }

        public void Press45MoveLeft(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Move(buttonRectTransform, buttonRectTransform.anchoredPosition, buttonRectTransform.anchoredPosition - new Vector2(20f, 0)));
        }

        public void Press46MoveRight(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(Move(buttonRectTransform, buttonRectTransform.anchoredPosition, buttonRectTransform.anchoredPosition + new Vector2(20f, 0)));
        }

        public void Press47ZigZag(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ZigZagEffect(buttonRectTransform));
        }

        public void Press48MoveUpAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(MoveAndBack(buttonRectTransform, buttonRectTransform.anchoredPosition, buttonRectTransform.anchoredPosition + new Vector2(0, 20f)));
        }

        public void Press49MoveDownAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(MoveAndBack(buttonRectTransform, buttonRectTransform.anchoredPosition, buttonRectTransform.anchoredPosition - new Vector2(0, 20f)));
        }

        public void Press50MoveLeftAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(MoveAndBack(buttonRectTransform, buttonRectTransform.anchoredPosition, buttonRectTransform.anchoredPosition - new Vector2(20f, 0)));
        }

        public void Press51MoveRightAndBack(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(MoveAndBack(buttonRectTransform, buttonRectTransform.anchoredPosition, buttonRectTransform.anchoredPosition + new Vector2(20f, 0)));
        }

        // --- CIRCLE EFFECTS ---
        public void Press52CircleSmall(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(CircleEffect(buttonRectTransform, 30f, 1f));
        }

        public void Press53CircleMedium(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(CircleEffect(buttonRectTransform, 30f, -1f));
        }

        public void Press54CircleLarge(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(CircleEffect(buttonRectTransform, 45f, 2.5f));
        }

        public void Press55CircleSlow(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(CircleEffect(buttonRectTransform, 45f, -2.5f));
        }

        public void Press56CircleFast(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(CircleEffect(buttonRectTransform, 20f, 1f));
        }

        public void Press57CircleReverse(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(CircleEffect(buttonRectTransform, 20f, -1f));
        }

        // --- SHAKE AND JIGGLE EFFECTS ---
        public void Press58Jiggle(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(JiggleEffect(buttonRectTransform));
        }

        public void Press59ShakeHorizontal(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ShakeEffect(buttonRectTransform, new Vector2(10f, 0)));
        }

        public void Press60ShakeVertical(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ShakeEffect(buttonRectTransform, new Vector2(0, 10f)));
        }

        public void Press61ShakeDiagonalRight(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ShakeEffect(buttonRectTransform, new Vector2(10f, 10f)));
        }

        public void Press62ShakeDiagonalLeft(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(ShakeEffect(buttonRectTransform, new Vector2(-10f, -10f)));
        }

        public void Press63Spring(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(SpringEffect(buttonRectTransform));
        }

        // --- OTHER EFFECTS ---
        public void Press64Flip(RectTransform buttonRectTransform)
        {
            if (!isAnimating) buttonRectTransform.localScale = new Vector3(buttonRectTransform.localScale.x * -1, buttonRectTransform.localScale.y, 1);
        }

        public void Press65FlipVertically(RectTransform buttonRectTransform)
        {
            if (!isAnimating) buttonRectTransform.localScale = new Vector3(buttonRectTransform.localScale.x, buttonRectTransform.localScale.y * -1, 1);
        }

        public void Press66Swing(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(SwingEffect(buttonRectTransform, 30f));
        }

        public void Press67FadeOut(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(FadeOutEffect(buttonRectTransform));
        }

        public void Press68RotateAndScaleAndBackRight(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateAndScaleAndBack(buttonRectTransform, 0f, 120f, Vector3.one, Vector3.one * 1.2f));
        }

        public void Press69RotateAndScaleAndBackLeft(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateAndScaleAndBack(buttonRectTransform, 0f, -120f, Vector3.one * 1.2f, Vector3.one));
        }

        public void Press70WobbleAndBackBig(RectTransform buttonRectTransform)
        {
            if (!isAnimating) StartCoroutine(RotateAndBack(buttonRectTransform, Random.Range(-75f, 75f), 0f));
        }

        // --- COROUTINE METHODS ---

        private IEnumerator RotateAndBack(RectTransform rectTransform, float fromAngle, float toAngle)
        {
            float elapsedTime = 0f;
            Quaternion originalRotation = rectTransform.localRotation; 

            while (elapsedTime < animationDuration)
            {
                float zRotation = Mathf.Lerp(fromAngle, toAngle, elapsedTime / animationDuration);
                rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.localRotation = Quaternion.Euler(0, 0, toAngle);

            yield return new WaitForSeconds(0.1f); 
            elapsedTime = 0f;
            
            while (elapsedTime < animationDuration)
            {
                float zRotation = Mathf.Lerp(toAngle, fromAngle, elapsedTime / animationDuration);
                rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.localRotation = originalRotation; 
        }

        private IEnumerator MoveAndBack(RectTransform rectTransform, Vector2 fromPosition, Vector2 toPosition)
        {
            Vector2 originalPosition = rectTransform.anchoredPosition;
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(fromPosition, toPosition, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.anchoredPosition = toPosition;

            yield return new WaitForSeconds(0.1f); 

            elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(toPosition, originalPosition, elapsedTime / animationDuration); 
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.anchoredPosition = originalPosition;
        }

        private IEnumerator PulseAndBack(RectTransform rectTransform, Vector3 fromScale, Vector3 toScale)
        {
            Vector3 originalScale = rectTransform.localScale;

            float elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                rectTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.localScale = toScale;

            yield return new WaitForSeconds(0.1f); 

            elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                rectTransform.localScale = Vector3.Lerp(toScale, originalScale, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.localScale = originalScale;
        }

        private IEnumerator RotateAndScaleAndBack(RectTransform rectTransform, float fromAngle, float toAngle, Vector3 fromScale, Vector3 toScale)
        {
            Quaternion originalRotation = rectTransform.localRotation;
            Vector3 originalScale = rectTransform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                float zRotation = Mathf.Lerp(fromAngle, toAngle, elapsedTime / animationDuration);
                rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);
                rectTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.localRotation = Quaternion.Euler(0, 0, toAngle);
            rectTransform.localScale = toScale;

            yield return new WaitForSeconds(0.1f);

            elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                float zRotation = Mathf.Lerp(toAngle, fromAngle, elapsedTime / animationDuration);
                rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);
                rectTransform.localScale = Vector3.Lerp(toScale, originalScale, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.localRotation = originalRotation;
            rectTransform.localScale = originalScale;
        }

        private IEnumerator RotateScalePulseEffect(RectTransform rectTransform)
        {
            Vector3 originalScale = rectTransform.localScale;
            float time = 0f;
            while (time < animationDuration)
            {
                float scale = Mathf.Lerp(1f, 1.5f, Mathf.PingPong(Time.time * 2f, 1f));
                float zRotation = Mathf.Lerp(0f, 45f, Mathf.PingPong(Time.time * 2f, 1f));
                rectTransform.localScale = new Vector3(scale, scale, 1f);
                rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);
                time += Time.deltaTime;
                yield return null;
            }
            rectTransform.localScale = originalScale;
            rectTransform.localRotation = Quaternion.identity;
        }

        private IEnumerator RotateBackAndForth(RectTransform rectTransform, float angle)
        {
            float time = 0f;
            while (time < animationDuration)
            {
                float zRotation = Mathf.Sin(time * Mathf.PI * 4f) * angle;
                rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);
                time += Time.deltaTime;
                yield return null;
            }
            rectTransform.localRotation = Quaternion.identity;
        }

        private IEnumerator Scale(RectTransform rectTransform, Vector3 fromScale, Vector3 toScale)
        {
            Vector3 originalScale = rectTransform.localScale; 
            float elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                rectTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.localScale = toScale; 
        }

        private IEnumerator ScaleAndBack(RectTransform rectTransform, Vector3 fromScale, Vector3 toScale)
        {
            Vector3 originalScale = rectTransform.localScale; 
            float elapsedTime = 0f;
            
            while (elapsedTime < animationDuration)
            {
                rectTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            rectTransform.localScale = toScale; 

            
            elapsedTime = 0f;
            yield return new WaitForSeconds(0.1f);
            
            while (elapsedTime < animationDuration)
            {
                rectTransform.localScale = Vector3.Lerp(toScale, originalScale, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            rectTransform.localScale = originalScale; 
        }

        private IEnumerator Rotate(RectTransform rectTransform, float fromAngle, float toAngle)
        {
            float elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                float zRotation = Mathf.Lerp(fromAngle, toAngle, elapsedTime / animationDuration);
                rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.localRotation = Quaternion.Euler(0, 0, toAngle);
        }

        private IEnumerator Move(RectTransform rectTransform, Vector2 fromPosition, Vector2 toPosition)
        {
            Vector2 originalPosition = rectTransform.anchoredPosition; 
            float elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(fromPosition, toPosition, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.anchoredPosition = toPosition; 
        }

        private IEnumerator PulseEffect(RectTransform rectTransform, Vector3 startScale, Vector3 endScale)
        {
            yield return Scale(rectTransform, startScale, endScale);
            yield return Scale(rectTransform, endScale, startScale);
        }

            private IEnumerator BounceEffect(RectTransform rectTransform, Vector3 startScale, Vector3 endScale)
        {
            yield return Scale(rectTransform, startScale, endScale);
            yield return Scale(rectTransform, endScale, startScale);
        }

        private IEnumerator TemporaryShrink(RectTransform rectTransform, Vector3 shrinkScale)
        {
            Vector3 originalScale = rectTransform.localScale;
            yield return Scale(rectTransform, originalScale, shrinkScale);
            yield return new WaitForSeconds(0.2f); 
            yield return Scale(rectTransform, shrinkScale, originalScale);
        }

        private IEnumerator CircleEffect(RectTransform rectTransform, float radius, float speed)
        {
            Vector2 originalPosition = rectTransform.anchoredPosition;
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration + 0.8f)
            {
                float angle = elapsedTime * speed * Mathf.PI * 2;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                rectTransform.anchoredPosition = originalPosition + new Vector2(x, y);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.anchoredPosition = originalPosition;
        }

        private IEnumerator ZigZagEffect(RectTransform rectTransform)
        {
            float elapsedTime = 0f;
            Vector2 originalPosition = rectTransform.anchoredPosition;
            while (elapsedTime < animationDuration + 1f)
            {
                float offsetX = Mathf.Sin(elapsedTime * Mathf.PI * 4f) * 20f * (elapsedTime / animationDuration + 1f); 
                rectTransform.anchoredPosition = new Vector2(originalPosition.x + offsetX, originalPosition.y);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.anchoredPosition = originalPosition; 
        }

        private IEnumerator TwistEffect(RectTransform rectTransform)
        {
            float time = 0f;
            while (time < animationDuration)
            {
                float zRotation = Mathf.Sin(time * Mathf.PI * 4f) * 15f;
                rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);
                time += Time.deltaTime;
                yield return null;
            }
            rectTransform.localRotation = Quaternion.identity;
        }

        private IEnumerator SpinEffect(RectTransform rectTransform, float rotationAngle, float spinDuration)
        {
            float time = 0f;
            while (time < spinDuration)
            {
                rectTransform.Rotate(0, 0, rotationAngle * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
            rectTransform.localRotation = Quaternion.identity; 
        }

        private IEnumerator SwingEffect(RectTransform rectTransform, float swingAngle)
        {
            isAnimating = true;
            float elapsedTime = 0f;
            float speedMultiplier = 2f; 

            Quaternion originalRotation = rectTransform.localRotation;

            while (elapsedTime < animationDuration + 1f)
            {
                float angle = Mathf.Sin(elapsedTime * Mathf.PI * speedMultiplier) * swingAngle;
                rectTransform.localRotation = Quaternion.Euler(0, angle, 0);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.localRotation = originalRotation;
            isAnimating = false; 
        }

        private IEnumerator JiggleEffect(RectTransform rectTransform)
        {
            Vector2 originalPosition = rectTransform.anchoredPosition;
            float elapsedTime = 0f;
            while (elapsedTime < animationDuration + 1f)
            {
                float offset = Mathf.Sin(elapsedTime * Mathf.PI * 8f) * 10f; 
                rectTransform.anchoredPosition = new Vector2(originalPosition.x + offset, originalPosition.y);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.anchoredPosition = originalPosition; 
        }

        private IEnumerator ShakeEffect(RectTransform rectTransform, Vector2 direction)
        {
            Vector2 originalPosition = rectTransform.anchoredPosition;
            float elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                rectTransform.anchoredPosition = originalPosition + new Vector2(Random.Range(-direction.x, direction.x), Random.Range(-direction.y, direction.y));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            rectTransform.anchoredPosition = originalPosition;
        }

        private IEnumerator SpringEffect(RectTransform rectTransform)
        {
            float time = 0f;
            while (time < animationDuration)
            {
                float scale = Mathf.Abs(Mathf.Sin(time * Mathf.PI * 4f)) * 0.5f + 1f;
                rectTransform.localScale = new Vector3(scale, scale, 1);
                time += Time.deltaTime;
                yield return null;
            }
            rectTransform.localScale = Vector3.one;
        }

        private IEnumerator FadeOutEffect(RectTransform rectTransform)
        {
            isAnimating = true; 
            CanvasGroup canvasGroup = rectTransform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>(); 
            }

            float elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 0f; 

            isAnimating = false; 
        }
    }
}