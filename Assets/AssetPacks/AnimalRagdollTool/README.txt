
Thanks you for trying my animal ragdoll tool for Unity!

All tool documentation is located in the tool by clicking the question mark
icon located at the top right of the tool.

Installation:
	
	Download and import into your unity project.
	
	
Using the tool:

	From the battleCrafters menu, select the Animal Ragdoll Tool option to
	start the tool.
	
	Open the test scene.
	
	Place the animal model you want to create a ragdoll for in the scene
	and in the center.  The models you use MUST already be rigged for Unity.
	
	Select the configuration you want to use that most resembles your
	animal or copy/create a new one.
	
	Place the transforms from the animal into the specified rig transform
	fields.
	
	When all required fields are set, click the 'Create' button at the top
	of the Rigs Panel.
	
	
Testing the ragdoll:

	At this point you can test your ragdoll.  If the model has an Animator
	component on it, disable it now.
	
	Run the scene.
	
	Click in the scene to add some explosion force to throw your ragdoll
	around.
	
	If your ragdoll breaks apart, then there is too much force being
	applied to the click.  In the test scenes hierarchy, click the
	AnimalRagdollTest object and lower the Explode Force value.
	Alternatively, you can try to increase the Total Mass of the ragdoll and
	create it again.  Ideally, you want to leave the Explode Force value to
	the common value you may use in your game and then adjust the Total
	Mass value.
	
	If the ragdoll is not flying around enough, do the opposite of above.
	
	
Tweaking the ragdoll:

	You can tweak the ragdoll by either editing the config values on the
	rigs or do in manually in the Collider, Rigidbody or ConfigurableJoint
	components of the animals transform.
	
	Keep in mind that values you change manually will not be saved in the
	configuration when you remove the ragdoll.
	
	To tweak the ragdoll, first click the Remove button to remove the
	ragdoll from the model.
	
	Make any changes you want on the rigs in question.
	
	Create the ragdoll again and test.
	