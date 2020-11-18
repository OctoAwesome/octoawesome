namespace OctoAwesome.OctoMath
{
    public class Polynomial
    {
        private readonly float[] coeffecients;

        public Polynomial(params float[] coeffecients)
            => this.coeffecients = coeffecients;

        public float Evaluate(float px)
        {
            if (coeffecients.Length == 0)
            {
                return 0;
            }

            var result = coeffecients[0];
            float x = px;
            for (var i = 1; i < coeffecients.Length; ++i)
            {
                result += x * coeffecients[i];
                x *= px;
            }

            return result;
        }
    }
}
