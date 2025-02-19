import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Checkbox } from "@/components/ui/checkbox"

export function ProjectCard() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Создать проект</CardTitle>
        <CardDescription>Проект - основа для управления задачами. Содержит задачи. 
          На проект назначаются пользователи с определенными ролями. 
          Для проекта можно задать список версий и список доступных категорий(модулей/подпроектов)</CardDescription>
      </CardHeader>
      <CardContent>
        <form>
          <div className="grid w-full items-center gap-4">
            <div className="flex flex-col space-y-2">
              <Label htmlFor="name">Имя проекта</Label>
              <Input id="name" placeholder="Задайте имя проекта" />
            </div>
            <div className="flex flex-col space-y-2">
              <Label htmlFor="description">Описание проекта</Label>
		          <Textarea />      
            </div>
            <div className="flex flex-col space-y-2">
                <Label htmlFor="parentProject">Родительский проект</Label>
                <Select>
                  <SelectTrigger id="framework">
                    <SelectValue placeholder="Select" />
                  </SelectTrigger>
                  <SelectContent position="popper" className="bg-white">
                    <SelectItem value="none">Корневой</SelectItem>
                    <SelectItem value="project1">Project 1</SelectItem>
                    <SelectItem value="project2">Project 2</SelectItem>
                    <SelectItem value="project3">Project 3</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="flex flex-row space-y-2 gap-4">
                <Checkbox />
                <label htmlFor="terms1">Наследовать участников
                </label>
              </div>
          </div>
        </form>
     </CardContent>
      <CardFooter className="flex justify-between">
        <Button id="saveButton">Сохранить</Button>
      </CardFooter>
    </Card>
  )
}