<Effect>
    <Technique name="SkyBox">
        <Pass name="Pass1">
            <Shader type="PixelShader" filename="skybox/skybox.ps">

            </Shader>
            <Shader type="VertexShader" filename="skybox/skybox.vs">

            </Shader>

            <Attributes>
                <attribute name="position">Position</attribute>
                <attribute name="color">Color</attribute>
                <attribute name="textureCoordinate">TextureCoordinate</attribute>
            </Attributes>
        </Pass>
    </Technique>
</Effect>
