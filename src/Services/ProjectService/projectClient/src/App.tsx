import { ProjectCard } from "./ProjectCard"
import { ProjectPage } from "./ProjectPage"

 
export default function Home() {
  return (
    <div className="flex h-screen w-screen ">
        <div className="p-36 w-full">
          <ProjectPage /> 
        </div>
   </div>
  )
}