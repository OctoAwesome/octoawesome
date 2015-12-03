<Effect>
    <Technique name="Ambient">
        <Pass name="Pass1">
            <Shader type="PixelShader" filename="simple/simple.ps">

            </Shader>
            <Shader type="VertexShader" filename="simple/simple.vs">
                <attribute name="position">Position</attribute>
                <attribute name="normal">Normal</attribute>
                <attribute name="texCoord">TexCoord</attribute>
            </Shader>
        </Pass>
    </Technique>
</Effect>