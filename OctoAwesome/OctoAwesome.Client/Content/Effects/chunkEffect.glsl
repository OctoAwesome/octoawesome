<Effect>
    <Technique name="Ambient">
        <Pass name="MainPass">
            <Shader type="PixelShader" filename="chunk/chunkEffect.ps">

            </Shader>
            <Shader type="VertexShader" filename="chunk/chunkEffect.vs">

            </Shader>
            <Attributes>
                <attribute name="inputData1">Position</attribute>
              <attribute name="inputData2">Normal</attribute>
            </Attributes>
        </Pass>
    </Technique>
    <Technique name="Shadow">
        <Pass name="MainPass">
            <Shader type="PixelShader" filename="chunk/chunkEffect_shadow.ps">

            </Shader>
            <Shader type="VertexShader" filename="chunk/chunkEffect_shadow.vs">

            </Shader>
            <Attributes>
                <attribute name="inputData1">Position</attribute>
              <attribute name="inputData2">Normal</attribute>
            </Attributes>
        </Pass>
    </Technique>
</Effect>
