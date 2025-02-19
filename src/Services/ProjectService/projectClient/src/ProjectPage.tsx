import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { ProjectCard } from "./ProjectCard"
import { UsersList } from "./UsersList"
import { VersionsList } from "./VersionsList"
import { CategoriesList } from "./CategoriesList"

export function ProjectPage() {

	return (
		<Tabs defaultValue="project" className="flex flex-col">
 	 		<TabsList>
    			<TabsTrigger value="project">Проект</TabsTrigger>
				<TabsTrigger value="users">Пользователи</TabsTrigger>
				<TabsTrigger value="versions">Версии</TabsTrigger>
				<TabsTrigger value="categories">Категории</TabsTrigger>
			</TabsList>
			<TabsContent value="project">
  				<ProjectCard />
			</TabsContent>
		  	<TabsContent value="users">
  				<UsersList />
			</TabsContent>
			<TabsContent value="versions">
  				<VersionsList />
			</TabsContent>
			<TabsContent value="categories">
  				<CategoriesList />
			</TabsContent>
		</Tabs>
	)
};