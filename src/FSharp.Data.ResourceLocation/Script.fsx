#I "../../packages/FParsec/lib/net40-client/"
#r "../../packages/FParsec/lib/net40-client/FParsecCS.dll"
#r "../../packages/FParsec/lib/net40-client/FParsec.dll"
#I "../../packages/Arachne.Core/lib/net45/"
#r "../../packages/Arachne.Core/lib/net45/Arachne.Core.dll"
#r "../../packages/Arachne.Uri/lib/net45/Arachne.Uri.dll"
#I "../../packages/Arachne.Uri/lib/net45/"
#r "../../packages/Arachne.Uri.Template/lib/net45/Arachne.Uri.Template.dll"

#r "../../bin/FSharp.Data.ResourceLocation/FSharp.Data.ResourceLocation.dll"



type somewhere = FSharp.Data.ResourceLocator<"http://ld.nice.org.uk/resource/{domain}#{id}">

