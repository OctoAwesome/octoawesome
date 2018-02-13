<Effect>
    <Technique name="BlockShadow">
        <Pass name="Pass1">
            <Shader type="PixelShader" filename="simple/blockShadow.ps">

            </Shader>
            <Shader type="VertexShader" filename="simple/blockShadow.vs">

            </Shader>
            <Attributes>
                <attribute name="inputData">Position</attribute>
              <attribute name="inputData2">Normal</attribute>
            </Attributes>
        </Pass>
    </Technique>
    <Technique name="EntityShadow">
            <Pass name="Pass1">
                <Shader type="PixelShader" filename="simple/entityShadow.ps">
    
                </Shader>
                <Shader type="VertexShader" filename="simple/entityShadow.vs">
    
                </Shader>
            </Pass>
        </Technique>
    <Technique name="BlockBasic">
        <Pass name="Pass1">
            <Shader type="PixelShader" filename="simple/blockSimple.ps">

            </Shader>
            <Shader type="VertexShader" filename="simple/blockSimple.vs">

            </Shader>
            <Attributes>
                <attribute name="inputData">Position</attribute>
              <attribute name="inputData2">Normal</attribute>
            </Attributes>
        </Pass>
    </Technique>
    <Technique name="EntityBasic">
        <Pass name="Pass1">
            <Shader type="PixelShader" filename="simple/entitySimple.ps">
    
            </Shader>
            <Shader type="VertexShader" filename="simple/entitySimple.vs">
    
            </Shader>
        </Pass>
    </Technique>
</Effect>
