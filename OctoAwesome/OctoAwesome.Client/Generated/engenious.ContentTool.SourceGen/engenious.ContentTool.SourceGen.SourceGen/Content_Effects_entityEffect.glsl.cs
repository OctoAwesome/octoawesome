
namespace engenious.UserDefined.Effects
{
    /// <summary>Implementation for the entityEffect effect.</summary>
    public partial class entityEffect : engenious.Graphics.Effect
    {
        /// <summary>Initializes a new instance of the <see cref="entityEffect"/> class.</summary>
        /// <param name="graphicsDevice">The graphics device for the effect.</param>
        public  entityEffect(engenious.Graphics.GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            
        }
        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();
            var techniques = Techniques;
            Ambient = techniques["Ambient"] as AmbientImpl;
            Shadow = techniques["Shadow"] as ShadowImpl;
        }
        /// <summary>Gets the <see cref="Ambient"/> technique.</summary>
        public AmbientImpl Ambient{ get; private set; }
        /// <summary>Gets the <see cref="Shadow"/> technique.</summary>
        public ShadowImpl Shadow{ get; private set; }
        /// <summary>Implementation for the Ambient effect technique</summary>
        public partial class AmbientImpl : engenious.Graphics.EffectTechnique
        {
            /// <summary>Initializes a new instance of the <see cref="AmbientImpl" /> class.</summary>
            /// <param name="name">The name of the effect technique.</param>
            public  AmbientImpl(string name)
                : base(name)
            {
                
            }
            /// <inheritdoc />
            protected override void Initialize()
            {
                base.Initialize();
                var passes = Passes;
                MainPass = passes["MainPass"] as MainPassImpl;
            }
            /// <summary>Gets the MainPass pass.</summary>
            public MainPassImpl MainPass{ get; private set; }
            /// <summary>Sets or gets the AmbientColor parameter.</summary>
            public engenious.Vector4 AmbientColor
            {
                get => MainPass.AmbientColor;
                set => MainPass.AmbientColor = value;
            }
            /// <summary>Sets or gets the AmbientIntensity parameter.</summary>
            public System.Single AmbientIntensity
            {
                get => MainPass.AmbientIntensity;
                set => MainPass.AmbientIntensity = value;
            }
            /// <summary>Gets the CascadeDepth parameter.</summary>
            public MainPassImpl.CascadeDepthArray CascadeDepth
            {
                get => MainPass.CascadeDepth;
            }
            /// <summary>Gets the CropMatrices parameter.</summary>
            public MainPassImpl.CropMatricesArray CropMatrices
            {
                get => MainPass.CropMatrices;
            }
            /// <summary>Sets or gets the DiffuseColor parameter.</summary>
            public engenious.Vector4 DiffuseColor
            {
                get => MainPass.DiffuseColor;
                set => MainPass.DiffuseColor = value;
            }
            /// <summary>Sets or gets the DiffuseDirection parameter.</summary>
            public engenious.Vector3 DiffuseDirection
            {
                get => MainPass.DiffuseDirection;
                set => MainPass.DiffuseDirection = value;
            }
            /// <summary>Sets or gets the DiffuseIntensity parameter.</summary>
            public System.Single DiffuseIntensity
            {
                get => MainPass.DiffuseIntensity;
                set => MainPass.DiffuseIntensity = value;
            }
            /// <summary>Sets or gets the Proj parameter.</summary>
            public engenious.Matrix Proj
            {
                get => MainPass.Proj;
                set => MainPass.Proj = value;
            }
            /// <summary>Sets or gets the ShadowMaps parameter.</summary>
            public engenious.Graphics.Texture2DArray ShadowMaps
            {
                get => MainPass.ShadowMaps;
                set => MainPass.ShadowMaps = value;
            }
            /// <summary>Sets or gets the Texture parameter.</summary>
            public engenious.Graphics.Texture2D Texture
            {
                get => MainPass.Texture;
                set => MainPass.Texture = value;
            }
            /// <summary>Sets or gets the View parameter.</summary>
            public engenious.Matrix View
            {
                get => MainPass.View;
                set => MainPass.View = value;
            }
            /// <summary>Sets or gets the World parameter.</summary>
            public engenious.Matrix World
            {
                get => MainPass.World;
                set => MainPass.World = value;
            }
            /// <summary>Implementation of the <see cref="MainPass"/>effect pass.</summary>
            public partial class MainPassImpl : engenious.Graphics.EffectPass
            {
                private engenious.Graphics.EffectPassParameter _AmbientColorPassParameter;
                private engenious.Vector4 _AmbientColor;
                private engenious.Graphics.EffectPassParameter _AmbientIntensityPassParameter;
                private System.Single _AmbientIntensity;
                private CascadeDepthArray _CascadeDepth;
                private CropMatricesArray _CropMatrices;
                private engenious.Graphics.EffectPassParameter _DiffuseColorPassParameter;
                private engenious.Vector4 _DiffuseColor;
                private engenious.Graphics.EffectPassParameter _DiffuseDirectionPassParameter;
                private engenious.Vector3 _DiffuseDirection;
                private engenious.Graphics.EffectPassParameter _DiffuseIntensityPassParameter;
                private System.Single _DiffuseIntensity;
                private engenious.Graphics.EffectPassParameter _ProjPassParameter;
                private engenious.Matrix _Proj;
                private engenious.Graphics.EffectPassParameter _ShadowMapsPassParameter;
                private engenious.Graphics.Texture2DArray _ShadowMaps;
                private engenious.Graphics.EffectPassParameter _TexturePassParameter;
                private engenious.Graphics.Texture2D _Texture;
                private engenious.Graphics.EffectPassParameter _ViewPassParameter;
                private engenious.Matrix _View;
                private engenious.Graphics.EffectPassParameter _WorldPassParameter;
                private engenious.Matrix _World;
                /// <summary>Initializes a new instance of the <see cref="MainPassImpl"/> class.</summary>
                public  MainPassImpl(engenious.Graphics.GraphicsDevice graphicsDevice, string name)
                    : base(graphicsDevice, name)
                {
                }
                /// <inheritdoc />
                protected override void CacheParameters()
                {
                    base.CacheParameters();
                    var parameters = Parameters;
                    _AmbientColorPassParameter = parameters["AmbientColor"];
                    _AmbientIntensityPassParameter = parameters["AmbientIntensity"];
                    _CascadeDepth = new (this, parameters["CascadeDepth[0]"]);
                    _CropMatrices = new (this, parameters["CropMatrices[0]"]);
                    _DiffuseColorPassParameter = parameters["DiffuseColor"];
                    _DiffuseDirectionPassParameter = parameters["DiffuseDirection"];
                    _DiffuseIntensityPassParameter = parameters["DiffuseIntensity"];
                    _ProjPassParameter = parameters["Proj"];
                    _ShadowMapsPassParameter = parameters["ShadowMaps"];
                    _TexturePassParameter = parameters["Texture"];
                    _ViewPassParameter = parameters["View"];
                    _WorldPassParameter = parameters["World"];
                }
                /// <summary>Gets or sets the AmbientColor parameter.</summary>
                public engenious.Vector4 AmbientColor
                {
                    get => _AmbientColor;
                    set
                    {
                        if (_AmbientColor == value)
                            return;
                        _AmbientColor = value;
                        _AmbientColorPassParameter.SetValue(value);
                    }

                }
                /// <summary>Gets or sets the AmbientIntensity parameter.</summary>
                public System.Single AmbientIntensity
                {
                    get => _AmbientIntensity;
                    set
                    {
                        if (_AmbientIntensity == value)
                            return;
                        _AmbientIntensity = value;
                        _AmbientIntensityPassParameter.SetValue(value);
                    }

                }
                /// <summary>Gets the CascadeDepth array values.</summary>
                public CascadeDepthArray CascadeDepth
                {
                    get => _CascadeDepth;
                }
                /// <summary>Gets the CropMatrices array values.</summary>
                public CropMatricesArray CropMatrices
                {
                    get => _CropMatrices;
                }
                /// <summary>Gets or sets the DiffuseColor parameter.</summary>
                public engenious.Vector4 DiffuseColor
                {
                    get => _DiffuseColor;
                    set
                    {
                        if (_DiffuseColor == value)
                            return;
                        _DiffuseColor = value;
                        _DiffuseColorPassParameter.SetValue(value);
                    }

                }
                /// <summary>Gets or sets the DiffuseDirection parameter.</summary>
                public engenious.Vector3 DiffuseDirection
                {
                    get => _DiffuseDirection;
                    set
                    {
                        if (_DiffuseDirection == value)
                            return;
                        _DiffuseDirection = value;
                        _DiffuseDirectionPassParameter.SetValue(value);
                    }

                }
                /// <summary>Gets or sets the DiffuseIntensity parameter.</summary>
                public System.Single DiffuseIntensity
                {
                    get => _DiffuseIntensity;
                    set
                    {
                        if (_DiffuseIntensity == value)
                            return;
                        _DiffuseIntensity = value;
                        _DiffuseIntensityPassParameter.SetValue(value);
                    }

                }
                /// <summary>Gets or sets the Proj parameter.</summary>
                public engenious.Matrix Proj
                {
                    get => _Proj;
                    set
                    {
                        if (_Proj == value)
                            return;
                        _Proj = value;
                        _ProjPassParameter.SetValue(value);
                    }

                }
                /// <summary>Gets or sets the ShadowMaps parameter.</summary>
                public engenious.Graphics.Texture2DArray ShadowMaps
                {
                    get => _ShadowMaps;
                    set
                    {
                        if (_ShadowMaps == value || (value != null && value.Equals(_ShadowMaps)))
                            return;
                        _ShadowMaps = value;
                        _ShadowMapsPassParameter.SetValue(value);
                    }

                }
                /// <summary>Gets or sets the Texture parameter.</summary>
                public engenious.Graphics.Texture2D Texture
                {
                    get => _Texture;
                    set
                    {
                        if (_Texture == value || (value != null && value.Equals(_Texture)))
                            return;
                        _Texture = value;
                        _TexturePassParameter.SetValue(value);
                    }

                }
                /// <summary>Gets or sets the View parameter.</summary>
                public engenious.Matrix View
                {
                    get => _View;
                    set
                    {
                        if (_View == value)
                            return;
                        _View = value;
                        _ViewPassParameter.SetValue(value);
                    }

                }
                /// <summary>Gets or sets the World parameter.</summary>
                public engenious.Matrix World
                {
                    get => _World;
                    set
                    {
                        if (_World == value)
                            return;
                        _World = value;
                        _WorldPassParameter.SetValue(value);
                    }

                }
                /// <summary>Wrapper class for the <c>CascadeDepth</c> array.</summary>
                public partial class CascadeDepthArray
                {
                    /// <summary>Initializes a new instance of the <see cref="CascadeDepthArray"/> class.</summary>
                    /// <param name="pass">The parent effect pass.</param>
                    /// <param name="parameter">The corresponding parameter.</param>
                    public  CascadeDepthArray(engenious.Graphics.EffectPass pass, engenious.Graphics.EffectPassParameter parameter)
                    {
                        Pass = pass;
                        Offset = parameter.Location;
                        Length = parameter.Size;
                        
                    }
                    /// <summary>Gets the length of the array.</summary>
                    public int Length
                    {
                        get;
                    }
                    /// <summary>Gets the layout size.</summary>
                    public int LayoutSize
                    {
                        get => Length;
                    }
                    /// <summary>Gets the parent effect pass.</summary>
                    public engenious.Graphics.EffectPass Pass
                    {
                        get;
                    }
                    /// <summary>Gets or sets the offset into the buffer.</summary>
                    public int Offset{ get; set; }
                    /// <summary>Sets the value at the given <paramref name="index" />.</summary>
/// <param name="index">The index to set the value at.</param>.
                    public System.Single this[int index]
                    {
                        set
                        {
                            if (index < 0 || index >= Length)
                                throw new System.ArgumentOutOfRangeException(nameof(index));
                            engenious.Graphics.EffectPassParameter.SetValue(Pass, Offset + index, value);
                        }

                    }
                }
                /// <summary>Wrapper class for the <c>CropMatrices</c> array.</summary>
                public partial class CropMatricesArray
                {
                    /// <summary>Initializes a new instance of the <see cref="CropMatricesArray"/> class.</summary>
                    /// <param name="pass">The parent effect pass.</param>
                    /// <param name="parameter">The corresponding parameter.</param>
                    public  CropMatricesArray(engenious.Graphics.EffectPass pass, engenious.Graphics.EffectPassParameter parameter)
                    {
                        Pass = pass;
                        Offset = parameter.Location;
                        Length = parameter.Size;
                        
                    }
                    /// <summary>Gets the length of the array.</summary>
                    public int Length
                    {
                        get;
                    }
                    /// <summary>Gets the layout size.</summary>
                    public int LayoutSize
                    {
                        get => Length;
                    }
                    /// <summary>Gets the parent effect pass.</summary>
                    public engenious.Graphics.EffectPass Pass
                    {
                        get;
                    }
                    /// <summary>Gets or sets the offset into the buffer.</summary>
                    public int Offset{ get; set; }
                    /// <summary>Sets the value at the given <paramref name="index" />.</summary>
/// <param name="index">The index to set the value at.</param>.
                    public engenious.Matrix this[int index]
                    {
                        set
                        {
                            if (index < 0 || index >= Length)
                                throw new System.ArgumentOutOfRangeException(nameof(index));
                            engenious.Graphics.EffectPassParameter.SetValue(Pass, Offset + index, value);
                        }

                    }
                }
            }
        }
        /// <summary>Implementation for the Shadow effect technique</summary>
        public partial class ShadowImpl : engenious.Graphics.EffectTechnique
        {
            /// <summary>Initializes a new instance of the <see cref="ShadowImpl" /> class.</summary>
            /// <param name="name">The name of the effect technique.</param>
            public  ShadowImpl(string name)
                : base(name)
            {
                
            }
            /// <inheritdoc />
            protected override void Initialize()
            {
                base.Initialize();
                var passes = Passes;
                MainPass = passes["MainPass"] as MainPassImpl;
            }
            /// <summary>Gets the MainPass pass.</summary>
            public MainPassImpl MainPass{ get; private set; }
            /// <summary>Gets the CropMatrices parameter.</summary>
            public MainPassImpl.CropMatricesArray CropMatrices
            {
                get => MainPass.CropMatrices;
            }
            /// <summary>Sets or gets the World parameter.</summary>
            public engenious.Matrix World
            {
                get => MainPass.World;
                set => MainPass.World = value;
            }
            /// <summary>Implementation of the <see cref="MainPass"/>effect pass.</summary>
            public partial class MainPassImpl : engenious.Graphics.EffectPass
            {
                private CropMatricesArray _CropMatrices;
                private engenious.Graphics.EffectPassParameter _WorldPassParameter;
                private engenious.Matrix _World;
                /// <summary>Initializes a new instance of the <see cref="MainPassImpl"/> class.</summary>
                public  MainPassImpl(engenious.Graphics.GraphicsDevice graphicsDevice, string name)
                    : base(graphicsDevice, name)
                {
                }
                /// <inheritdoc />
                protected override void CacheParameters()
                {
                    base.CacheParameters();
                    var parameters = Parameters;
                    _CropMatrices = new (this, parameters["CropMatrices[0]"]);
                    _WorldPassParameter = parameters["World"];
                }
                /// <summary>Gets the CropMatrices array values.</summary>
                public CropMatricesArray CropMatrices
                {
                    get => _CropMatrices;
                }
                /// <summary>Gets or sets the World parameter.</summary>
                public engenious.Matrix World
                {
                    get => _World;
                    set
                    {
                        if (_World == value)
                            return;
                        _World = value;
                        _WorldPassParameter.SetValue(value);
                    }

                }
                /// <summary>Wrapper class for the <c>CropMatrices</c> array.</summary>
                public partial class CropMatricesArray
                {
                    /// <summary>Initializes a new instance of the <see cref="CropMatricesArray"/> class.</summary>
                    /// <param name="pass">The parent effect pass.</param>
                    /// <param name="parameter">The corresponding parameter.</param>
                    public  CropMatricesArray(engenious.Graphics.EffectPass pass, engenious.Graphics.EffectPassParameter parameter)
                    {
                        Pass = pass;
                        Offset = parameter.Location;
                        Length = parameter.Size;
                        
                    }
                    /// <summary>Gets the length of the array.</summary>
                    public int Length
                    {
                        get;
                    }
                    /// <summary>Gets the layout size.</summary>
                    public int LayoutSize
                    {
                        get => Length;
                    }
                    /// <summary>Gets the parent effect pass.</summary>
                    public engenious.Graphics.EffectPass Pass
                    {
                        get;
                    }
                    /// <summary>Gets or sets the offset into the buffer.</summary>
                    public int Offset{ get; set; }
                    /// <summary>Sets the value at the given <paramref name="index" />.</summary>
/// <param name="index">The index to set the value at.</param>.
                    public engenious.Matrix this[int index]
                    {
                        set
                        {
                            if (index < 0 || index >= Length)
                                throw new System.ArgumentOutOfRangeException(nameof(index));
                            engenious.Graphics.EffectPassParameter.SetValue(Pass, Offset + index, value);
                        }

                    }
                }
            }
        }
        /// <summary>Implementation for the effect settings.</summary>
        public partial record entityEffectSettings : engenious.Graphics.IEffectSettings
        {
            /// <inheritdoc />
            public string ToCode()
             => $"#define CASCADES {CASCADES}\n"
            ;/// <summary>Gets or sets the CASCADES setting.</summary>
            public uint CASCADES
            {
                get => Shadow.CASCADES;
                set
                {
                    Shadow.CASCADES = value;
                }

            }
            public ShadowSettings Shadow{ get; set; } = new ();
            /// <summary>Implementation for the effect settings.</summary>
            public partial record ShadowSettings : engenious.Graphics.IEffectSettings
            {
                /// <inheritdoc />
                public string ToCode()
                 => $"#define CASCADES {CASCADES}\n"
                ;/// <summary>Gets or sets the CASCADES setting.</summary>
                public uint CASCADES{ get; set; } = 2;
            }
        }
    }
}
