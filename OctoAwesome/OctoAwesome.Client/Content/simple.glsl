<Effect>
    <Technique name="Ambient">
        <Pass name="Pass1">
            <Shader type="PixelShader" filename="simple/simple.ps">

            </Shader>
            <Shader type="VertexShader" filename="simple/simple.vs">

            </Shader>
            <Attributes>
                <attribute name="inputData">Position</attribute>
              <attribute name="inputData2">Normal</attribute>
            </Attributes>
        </Pass>
    </Technique>
    <Technique name="water">
        <Pass name="Pass1">
            <Shader type="PixelShader" filename="simple/simpleWater.ps">

            </Shader>
            <Shader type="VertexShader" filename="simple/simpleWater.vs">

            </Shader>
            <Attributes>
                <attribute name="inputData">Position</attribute>
              <attribute name="inputData2">Normal</attribute>
            </Attributes>
        </Pass>
    </Technique>
</Effect>
