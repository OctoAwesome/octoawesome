<Effect>
	<Technique name="Ambient">
		<Pass name="MainPass">
			<Shader type="PixelShader" filename="chunk/chunkEffect.ps"/>
			<Shader type="VertexShader" filename="chunk/chunkEffect.vs"/>
			<Attributes>
				<attribute name="inputData1">Position</attribute>
				<attribute name="inputData2">Normal</attribute>
			</Attributes>
		</Pass>
	</Technique>
	<Technique name="Shadow">
		<Pass name="MainPass">
			<Shader type="PixelShader" filename="chunk/chunkEffect_shadow.ps"/>
			<Shader type="VertexShader" filename="chunk/chunkEffect_shadow.vs"/>
			<Attributes>
				<attribute name="inputData1">Position</attribute>
				<attribute name="inputData2">Normal</attribute>
			</Attributes>
		</Pass>
	</Technique>
	<Technique name="Transparent">
		<Pass name="MainPass">
			<Shader type="PixelShader" filename="chunk/chunkEffect_transparent.ps"/>
			<Shader type="VertexShader" filename="chunk/chunkEffect_transparent.vs" />
			<Attributes>
				<attribute name="inputData1">Position</attribute>
				<attribute name="inputData2">Normal</attribute>
			</Attributes>
		</Pass>
	</Technique>
</Effect>
