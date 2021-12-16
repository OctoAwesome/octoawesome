<Effect>
    <Technique name="Ambient">
        <Pass name="MainPass">
            <Shader type="PixelShader" filename="entity/entityEffect.ps"></Shader>
            <Shader type="VertexShader" filename="entity/entityEffect.vs"></Shader>
            <Attributes>
                <attribute name="position">Position</attribute>
                <attribute name="normal">Normal</attribute>
                <attribute name="textureCoord">TextureCoordinate</attribute>
            </Attributes>
        </Pass>
    </Technique>
    <Technique name="Shadow">
        <Pass name="MainPass">
            <Shader type="PixelShader" filename="entity/entityEffect_shadow.ps"></Shader>
            <Shader type="VertexShader" filename="entity/entityEffect_shadow.vs"></Shader>
            <Attributes>
                <attribute name="position">Position</attribute>
                <attribute name="normal">Normal</attribute>
                <attribute name="textureCoord">TextureCoordinate</attribute>
            </Attributes>
        </Pass>
    </Technique>
</Effect>