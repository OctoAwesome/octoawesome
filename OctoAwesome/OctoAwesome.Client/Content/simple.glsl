<Effect>
    <Technique name="Shadow">
            <Pass name="Pass1">
                <Shader type="PixelShader" filename="simple/shadow.ps">
    
                </Shader>
                <Shader type="VertexShader" filename="simple/shadow.vs">
    
                </Shader>
                <Attributes>
                    <attribute name="inputData">Position</attribute>
                  <attribute name="inputData2">Normal</attribute>
                </Attributes>
            </Pass>
        </Technique>
    <Technique name="Basic">
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
</Effect>
