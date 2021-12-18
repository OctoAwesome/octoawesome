namespace OctoAwesome.OctoMath
{
    /// <summary>
    /// For calculating polynomial functions.
    /// </summary>
    public class Polynomial
    {
        private readonly float[] coefficients;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polynomial"/> class.
        /// </summary>
        /// <param name="coefficients">The polynomial coefficients.</param>
        public Polynomial(params float[] coefficients)
            => this.coefficients = coefficients;

        /// <summary>
        /// Evaluates the polynomial at a given position.
        /// </summary>
        /// <param name="px">The position to evaluate the polynomial at.</param>
        /// <returns>The evaluated value.</returns>
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
