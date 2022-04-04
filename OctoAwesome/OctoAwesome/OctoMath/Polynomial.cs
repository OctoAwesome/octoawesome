namespace OctoAwesome.OctoMath
{

    public class Polynomial
    {
        private readonly float[] coefficients;

        public Polynomial(params float[] coefficients)
            => this.coefficients = coefficients;
        public float Evaluate(float px)
        {
            if (coefficients.Length == 0)
            {
                return 0;
            }

            // c0 + c1 * x + c2 * x^2
            var result = coefficients[0];
            float x = px;
            for (var i = 1; i < coefficients.Length; ++i)
            {
                result += x * coefficients[i];
                x *= px;
            }

            return result;
        }
    }
}
